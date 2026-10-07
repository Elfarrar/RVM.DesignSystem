# Gera o RvmFieldFrame.razor.css a partir do RvmTextField.razor.css (DSGN-017): python -I tools/moldura-do-campo.py
import os
import re

base = os.path.join(os.path.dirname(os.path.abspath(__file__)), "..", "src", "RVM.DesignSystem", "Components", "TextField")
s = open(os.path.join(base, "RvmTextField.razor.css"), encoding="utf-8").read()

FILHOS = (".rvm-entrada", ".rvm-afixo", ".rvm-icone-do-campo")


def sem_parenteses(t):
    while True:
        n = re.sub(r"\([^()]*\)", "", t)
        if n == t:
            return t
        t = n


def ajustar(seletor):
    # O alvo (ultimo composto) e o que recebe o atributo de escopo. Se ele e um elemento que o campo passa no
    # ChildContent, o escopo tem que ficar no ancestral: ::deep antes do alvo. O que esta dentro de :has()/:not()
    # nao conta como alvo.
    lead = seletor[:len(seletor) - len(seletor.lstrip())]
    corpo = seletor.strip()
    partes = re.split(r"(\s*[>~+]\s*|\s+)(?![^(]*\))", corpo)
    alvo = partes[-1]
    if not any(f in sem_parenteses(alvo) for f in FILHOS) or "::deep" in corpo:
        return seletor
    if len(partes) == 1:
        return lead + ".rvm-caixa ::deep " + alvo
    return lead + "".join(partes[:-2]) + " ::deep " + alvo


def processar(css):
    css = re.sub(r"/\*.*?\*/", "", css, flags=re.S)

    def troca(m):
        return ",".join(ajustar(x) for x in m.group(1).split(",")) + "{" + m.group(2) + "}"
    return re.sub(r"([^{}@]+)\{([^{}]*)\}", troca, css)


corpo = processar(s)
cabecalho = """/* CSS isolado do RvmFieldFrame — gerado do RvmTextField.razor.css (DSGN-017): a moldura e a mesma, e o controle
   (input, textarea, prefixo) vem no ChildContent do campo que a usa, por isso as regras cujo alvo e o controle levam
   ::deep. Mudou o visual do campo de texto? Regere este arquivo tambem.
"""
corpo = cabecalho + "*/" + chr(10) + chr(10) + corpo.strip() + chr(10)
corpo += """
/* --- So da moldura: o que o RvmTextField sabia pelos parametros, aqui vem do proprio controle --- */

/* Placeholder do consumidor (diferente do espaco tecnico): o rotulo fica sempre em cima. */
.rvm-contorno .rvm-caixa:has(.rvm-entrada:not([placeholder=" "])) .rvm-rotulo,
.rvm-contorno .rvm-caixa:has(.rvm-prefixo) .rvm-rotulo {
    top: 0;
    transform: translateY(-50%) scale(0.75);
    background-color: var(--rvm-textfield-notch-background, var(--rvm-color-background-paper));
}

.rvm-preenchido .rvm-caixa:has(.rvm-entrada:not([placeholder=" "])) .rvm-rotulo,
.rvm-preenchido .rvm-caixa:has(.rvm-prefixo) .rvm-rotulo {
    top: 8px;
    transform: scale(0.75);
}

.rvm-padrao .rvm-caixa:has(.rvm-entrada:not([placeholder=" "])) .rvm-rotulo,
.rvm-padrao .rvm-caixa:has(.rvm-prefixo) .rvm-rotulo {
    top: 2px;
    transform: scale(0.75);
}

/* Varias linhas (area de texto): o conteudo comeca no alto e o rotulo fica sempre em cima. */
.rvm-multilinha .rvm-caixa {
    align-items: stretch;
}

.rvm-multilinha .rvm-caixa ::deep .rvm-entrada {
    padding-block: 16px 12px;
    resize: vertical;
}

.rvm-contorno.rvm-multilinha .rvm-rotulo {
    top: 0;
    transform: translateY(-50%) scale(0.75);
    background-color: var(--rvm-textfield-notch-background, var(--rvm-color-background-paper));
}

.rvm-preenchido.rvm-multilinha .rvm-rotulo,
.rvm-padrao.rvm-multilinha .rvm-rotulo {
    top: 4px;
    transform: scale(0.75);
}

.rvm-grande .rvm-caixa { min-height: 64px; }

.rvm-so-leitor {
    position: absolute;
    width: 1px;
    height: 1px;
    overflow: hidden;
    clip: rect(0 0 0 0);
    white-space: nowrap;
}
"""
open(os.path.join(base, "RvmFieldFrame.razor.css"), "w", encoding="utf-8", newline="\n").write(corpo)
print(sum(1 for _ in re.finditer("::deep", corpo)))
