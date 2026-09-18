"""Le os tokens que o kit NEATLAB nao escreve, medindo os PNGs de `referencia-neatlab/`.

Por que existe: o arquivo do Figma Community e uma IMAGEM, nao objetos de desenho (confirmado pelo
Rafael em 18/09/2026) — nao ha valor para ler por API, so pixel para medir. Ver DSGN-014.

Uso:
    python tools/amostragem-do-kit.py sombras
    python tools/amostragem-do-kit.py texto
    python tools/amostragem-do-kit.py fundos

Nao roda no CI: e ferramenta de medicao, feita para ser repetida a mao quando a referencia mudar.
"""
import sys
from pathlib import Path

import numpy as np
from PIL import Image

RAIZ = Path(__file__).resolve().parent.parent
REFERENCIA = RAIZ / "referencia-neatlab"


def abrir(caminho: str) -> np.ndarray:
    """A imagem como float 0..1, ja achatada sobre branco (o alpha do PNG nao interessa aqui)."""
    imagem = Image.open(REFERENCIA / caminho).convert("RGBA")
    dados = np.asarray(imagem, dtype=np.float64) / 255.0
    alfa = dados[..., 3:4]
    return dados[..., :3] * alfa + (1 - alfa)


def luminancia(rgb: np.ndarray) -> np.ndarray:
    return rgb @ np.array([0.2126, 0.7152, 0.0722])


def hexa(rgb) -> str:
    return "#" + "".join(f"{round(float(c) * 255):02X}" for c in rgb)


# --------------------------------------------------------------------------------------- sombras

def faixas(projecao: np.ndarray, minimo: int, tamanho: int = 20) -> list[tuple[int, int]]:
    """Trechos contiguos em que a projecao passa do minimo — uma faixa por linha/coluna da grade.

    `tamanho` e o comprimento minimo do trecho: 20 px separa os quadrados da grade de sombras, mas uma
    linha de legenda tem 8 px de altura e sumiria com esse corte.
    """
    achadas: list[tuple[int, int]] = []
    dentro = None
    for i, valor in enumerate(projecao):
        if valor >= minimo and dentro is None:
            dentro = i
        elif valor < minimo and dentro is not None:
            if i - dentro >= tamanho:
                achadas.append((dentro, i - 1))
            dentro = None

    if dentro is not None and len(projecao) - dentro >= tamanho:
        achadas.append((dentro, len(projecao) - 1))

    return achadas


def caixas(luz: np.ndarray, limite: float = 0.985) -> list[tuple[int, int, int, int]]:
    """Os 24 quadrados brancos da tela Shadow, em ordem de leitura.

    Por projecao, e nao por regiao: os quadrados estao numa grade regular, e somar branco por linha e
    por coluna separa as 4 linhas das 6 colunas sem depender de o branco ser continuo.
    """
    altura, largura = luz.shape
    recorte = luz[altura // 2:, int(largura * 0.4):]
    branco = recorte > limite

    linhas = faixas(branco.sum(axis=1), minimo=branco.shape[1] // 20)
    colunas = faixas(branco.sum(axis=0), minimo=branco.shape[0] // 20)

    return [
        (x0 + int(largura * 0.4), y0 + altura // 2, x1 + int(largura * 0.4), y1 + altura // 2)
        for y0, y1 in linhas
        for x0, x1 in colunas
    ]


def perfil(luz: np.ndarray, fundo: float, pixels: np.ndarray) -> np.ndarray:
    """Quanto de preto ha em cada pixel: a sombra escurece o fundo, entao alpha = 1 - cor/fundo."""
    return np.clip(1 - pixels / fundo, 0, 1)


def normal_acumulada(z: np.ndarray) -> np.ndarray:
    """Phi(z) sem scipy: erf por aproximacao de Abramowitz-Stegun 7.1.26 (erro < 1.5e-7)."""
    x = z / np.sqrt(2)
    sinal = np.sign(x)
    x = np.abs(x)
    t = 1 / (1 + 0.3275911 * x)
    y = 1 - (((((1.061405429 * t - 1.453152027) * t) + 1.421413741) * t - 0.284496736) * t + 0.254829592) * t * np.exp(-x * x)
    return 0.5 * (1 + sinal * y)


def ajustar_borda(medido: np.ndarray, distancias: np.ndarray) -> tuple[float, float, float, float]:
    """(alpha, borda, sigma, erro) que melhor explicam o perfil de uma borda borrada, por busca em grade.

    A sombra do CSS e a forma deslocada e borrada: ao longo de uma linha que sai do quadrado, o perfil
    e a acumulada da normal. `borda` e onde a sombra "termina" (deslocamento +/- espalhamento) e
    `sigma` e o desfoque (o blur do CSS vale 2 sigma).
    """
    melhor = (0.0, 0.0, 1.0, float("inf"))
    for sigma in np.arange(0.5, 20.0, 0.25):
        for borda in np.arange(-10.0, 24.0, 0.25):
            forma = normal_acumulada((borda - distancias) / sigma)
            denominador = float(forma @ forma)
            if denominador < 1e-9:
                continue

            alpha = min(float(medido @ forma) / denominador, 1.0)
            erro = float(np.mean((medido - alpha * forma) ** 2))
            if erro < melhor[3]:
                melhor = (alpha, float(borda), float(sigma), erro)

    return melhor


def sombra_de(caixa, dy: float, sigma: float, forma_x: np.ndarray, forma_y: np.ndarray) -> np.ndarray:
    """A mancha de UMA sombra na grade: retangulo deslocado em dy e borrado com desvio sigma."""
    x0, y0, x1, y1 = caixa
    fx = normal_acumulada((forma_x - x0) / sigma) - normal_acumulada((forma_x - x1) / sigma)
    fy = normal_acumulada((forma_y - (y0 + dy)) / sigma) - normal_acumulada((forma_y - (y1 + dy)) / sigma)
    return np.outer(fy, fx)


def sombras(detalhe: bool = False) -> None:
    """Mede deslocamento, desfoque e opacidade das 24 elevacoes, ajustando TODAS de uma vez.

    Por que todas juntas: os quadrados tem 71 px com 25 px de folga, e as sombras grandes passam de 40 px
    — elas se sobrepoem no PNG. Medir uma de cada vez dava "espalhamento" de 20 px que nao existe, so
    porque a mancha do vizinho entrava na conta. Aqui cada quadrado e ajustado sobre o RESIDUO (a imagem
    menos a sombra de todos os outros), em algumas passadas, ate parar de mudar.

    Limites honestos desta medicao:
    - `spread` fica fixo em 0. Com as manchas sobrepostas, espalhamento e deslocamento nao se separam;
      escala de elevacao costuma usar spread 0, e o perfil medido e reproduzido sem ele.
    - O kit provavelmente empilha 2 ou 3 camadas de sombra por elevacao (padrao do Material). O ajuste e
      de UMA camada: o resultado e a sombra equivalente, nao a receita original.
    """
    rgb = abrir("Shadow/Light.png")
    luz = luminancia(rgb)
    quadrados = caixas(luz)
    if not quadrados:
        print("nao achei a grade de sombras")
        return

    margem = 60
    x_inicio = min(q[0] for q in quadrados) - margem
    x_fim = max(q[2] for q in quadrados) + margem
    y_inicio = min(q[1] for q in quadrados) - margem
    y_fim = max(q[3] for q in quadrados) + margem
    recorte = luz[y_inicio:y_fim, x_inicio:x_fim]
    fundo = float(np.median(luz[y_inicio:y_fim, max(0, x_inicio - 260):max(1, x_inicio - 60)]))

    # Quanto de preto ha em cada pixel do recorte, e onde nao ha quadrado branco por cima.
    medido = np.clip(1 - recorte / fundo, 0, 1)
    locais = [(x0 - x_inicio, y0 - y_inicio, x1 - x_inicio, y1 - y_inicio) for x0, y0, x1, y1 in quadrados]
    fora = np.ones_like(medido, dtype=bool)
    for x0, y0, x1, y1 in locais:
        fora[max(0, y0 - 1):y1 + 2, max(0, x0 - 1):x1 + 2] = False

    forma_x = np.arange(recorte.shape[1], dtype=np.float64)
    forma_y = np.arange(recorte.shape[0], dtype=np.float64)

    # Chute inicial: sombra pequena e clara para todos.
    parametros = [[2.0, 3.0, 0.15] for _ in locais]

    def mancha(i):
        dy, sigma, alpha = parametros[i]
        return alpha * sombra_de(locais[i], dy, sigma, forma_x, forma_y)

    for passada in range(4):
        mudou = 0.0
        soma = sum(mancha(i) for i in range(len(locais)))
        for i, (x0, y0, x1, y1) in enumerate(locais):
            resto = soma - mancha(i)
            # Sobra do que os outros nao explicam, na vizinhanca deste quadrado.
            alvo = np.clip(medido - resto, 0, 1)
            janela = np.zeros_like(fora)
            janela[max(0, y0 - 50):y1 + 50, max(0, x0 - 50):x1 + 50] = True
            usar = janela & fora

            melhor = (parametros[i], float("inf"))
            for dy in np.arange(0.0, 20.5, 0.5):
                for sigma in np.arange(0.5, 22.0, 0.5):
                    forma = sombra_de(locais[i], dy, sigma, forma_x, forma_y)[usar]
                    denominador = float(forma @ forma)
                    if denominador < 1e-9:
                        continue

                    alpha = min(float(alvo[usar] @ forma) / denominador, 1.0)
                    erro = float(np.mean((alvo[usar] - alpha * forma) ** 2))
                    if erro < melhor[1]:
                        melhor = ([float(dy), float(sigma), alpha], erro)

            mudou = max(mudou, abs(melhor[0][0] - parametros[i][0]) + abs(melhor[0][1] - parametros[i][1]))
            parametros[i] = melhor[0]
            soma = resto + mancha(i)

        if detalhe:
            print(f"passada {passada + 1}: maior mudanca {mudou:.2f} px")

        if mudou < 0.5:
            break

    soma = sum(mancha(i) for i in range(len(locais)))
    residuo = float(np.sqrt(np.mean((medido[fora] - soma[fora]) ** 2)))
    print(f"\n=== 24 elevacoes do kit (tela Shadow, tema claro) ===")
    print(f"residuo medio do ajuste: {residuo:.4f} de opacidade (0 a 1)\n")
    print("  #   dy    blur   alpha   CSS")
    for i, (dy, sigma, alpha) in enumerate(parametros, start=1):
        css = f"0 {dy:.0f}px {2 * sigma:.0f}px rgba(0, 0, 0, {alpha:.2f})"
        print(f"{i:3d}  {dy:5.1f}  {2 * sigma:5.1f}  {alpha:5.3f}  {css}")

    if detalhe:
        # O quadrado branco entra por cima: sem ele a comparacao mostraria a mancha inteira, que na tela
        # fica escondida atras da caixa.
        modelo = np.clip(1 - soma, 0, 1) * fundo
        for x0, y0, x1, y1 in locais:
            modelo[y0:y1 + 1, x0:x1 + 1] = 1.0

        prova = np.stack([modelo] * 3, axis=-1)
        Image.fromarray((np.concatenate([recorte[..., None].repeat(3, axis=-1), prova], axis=1) * 255)
                        .astype(np.uint8)).save(RAIZ / "docs/fotos/dsgn-014/sombras-medido-vs-modelo.png")
        print("\ncomparacao salva em docs/fotos/dsgn-014/sombras-medido-vs-modelo.png")


# ----------------------------------------------------------------------------------------- texto

def miolo_do_traco(mascara: np.ndarray) -> np.ndarray:
    """So os pixels de texto cercados por texto: tira o antialias, que inventa todos os tons."""
    dentro = mascara.copy()
    for dy in (-1, 0, 1):
        for dx in (-1, 0, 1):
            dentro &= np.roll(np.roll(mascara, dy, axis=0), dx, axis=1)

    return dentro


def niveis_de_texto(arquivo: str, comeco: float = 1 / 3) -> tuple[np.ndarray, float, list[tuple[float, np.ndarray, int]]]:
    """(cor do fundo, luminancia do fundo, niveis) de uma tela: cada nivel e (luminancia, cor, linhas).

    Medido por LINHA de texto: a borda das letras tem antialias e produz todos os tons intermediarios,
    e erodir o traco apagaria as legendas pequenas. De cada linha sai o pixel mais cheio, que e a cor
    de verdade daquele estilo.
    """
    rgb = abrir(arquivo)
    luz = luminancia(rgb)
    de = int(luz.shape[0] * comeco)
    corpo = luz[de:, :]
    valores, contagens = np.unique(np.round(corpo, 3), return_counts=True)
    fundo = float(valores[np.argmax(contagens)])
    cor_fundo = np.median(rgb[de:][np.abs(corpo - fundo) < 0.004], axis=0)

    claro = fundo > 0.5
    mascara = (corpo < fundo - 0.05) if claro else (corpo > fundo + 0.05)
    grupos: dict[float, list[np.ndarray]] = {}
    for y0, y1 in faixas(mascara.sum(axis=1), minimo=3, tamanho=5):
        pedaco = corpo[y0:y1 + 1]
        dentro = mascara[y0:y1 + 1]
        if dentro.sum() < 40:
            continue

        tom = float(np.percentile(pedaco[dentro], 2 if claro else 98))
        cheios = dentro & (np.abs(pedaco - tom) < 0.02)
        if not cheios.any():
            continue

        grupos.setdefault(round(tom, 2), []).append(np.median(rgb[de:][y0:y1 + 1][cheios], axis=0))

    niveis = [(tom, np.median(np.stack(cores), axis=0), len(cores)) for tom, cores in grupos.items()]
    niveis.sort(key=lambda n: -n[2])
    return cor_fundo, fundo, niveis


def texto(detalhe: bool = False) -> None:
    """As cores de cada nivel de enfase do texto, nos dois temas.

    O kit NAO usa branco/preto com opacidade: usa cor solida (medido). O percentual abaixo e so a
    equivalencia — quanto de branco (ou de preto) aquela cor representa sobre o fundo da tela.
    """
    telas = [("Tipografia", "Typography/Light.png", "Typography/Dark.png"),
             ("Botoes (tem estado desabilitado)", "Button.png", "Button-1.png")]
    for titulo, claro_png, escuro_png in telas:
        for tema, arquivo in [("claro", claro_png), ("escuro", escuro_png)]:
            if not (REFERENCIA / arquivo).exists():
                continue

            cor_fundo, fundo, niveis = niveis_de_texto(arquivo)
            print(f"\n=== {titulo} · tema {tema} · fundo {hexa(cor_fundo)} ===")
            for tom, cor, quantas in niveis[:6]:
                alvo = 1.0 if fundo < 0.5 else 0.0
                # Quanto de branco (tema escuro) ou de preto (tema claro) esta cor equivale sobre o fundo.
                equivalente = float(np.median((cor - cor_fundo) / (alvo - cor_fundo + 1e-9)))
                print(f"  {hexa(cor)} · luminancia {tom:.2f} · {quantas:3d} linhas · "
                      f"equivale a {'branco' if alvo else 'preto'} {equivalente * 100:4.0f}%")


# ---------------------------------------------------------------------------------------- fundos

def fundos(detalhe: bool = False) -> None:
    """Os fundos suaves do tema claro: sao roxos ou azuis? Mede o matiz de cada um."""
    import colorsys

    telas = ["Alert.png", "Advanced Card.png", "Accordion.png"]
    for tela in telas:
        caminho = REFERENCIA / tela
        if not caminho.exists():
            continue

        rgb = abrir(tela)
        achatado = rgb.reshape(-1, 3)
        # Cores claras e pouco saturadas: os fundos suaves de cartao, alerta e chip.
        cores, quantidades = np.unique((achatado * 255).round().astype(np.uint8), axis=0, return_counts=True)
        print(f"\n=== {tela} ===")
        for cor, quantidade in sorted(zip(cores, quantidades), key=lambda p: -p[1])[:14]:
            r, g, b = cor / 255
            matiz, luminosidade, saturacao = colorsys.rgb_to_hls(r, g, b)
            if luminosidade < 0.7 or saturacao < 0.05 or quantidade < 500:
                continue

            print(f"  {hexa(cor / 255)} · {quantidade:8d} px · matiz {matiz * 360:5.1f}° · "
                  f"saturacao {saturacao:.2f} · luz {luminosidade:.2f}")


if __name__ == "__main__":
    escolha = sys.argv[1] if len(sys.argv) > 1 else "sombras"
    {"sombras": sombras, "texto": texto, "fundos": fundos}[escolha](*([True] if "-v" in sys.argv else []))
