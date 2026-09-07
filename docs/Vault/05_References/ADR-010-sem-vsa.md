---
adr: 010
titulo: A biblioteca nao segue Vertical Slice Architecture
status: aceito
data: 2026-09-07
projeto: RVM.DesignSystem
---

# ADR-010 — Sem VSA neste projeto

## Contexto

O padrao do ecossistema RVM e **Vertical Slice Architecture + MediatR + dominio unico**,
espelhando o RVM.ERPAgro (skill `padrao-rvm` §1). Todo projeto novo herda esse alvo por padrao.

## Decisao

**Este projeto nao segue VSA.** A unidade de organizacao e o **componente**, e a estrutura de
pastas e por familia (`Basicos/`, `Layout/`, `Feedback/`, `Dados/`), como descrito em
`03-arquitetura.md` § Forma do artefato.

## Motivo

VSA organiza **casos de uso que atravessam camadas** — endpoint, handler, validacao, banco.
Aqui nao existe nenhuma das quatro coisas: o artefato e uma Razor Class Library. Nao ha
requisicao HTTP, nao ha persistencia, nao ha `DbContext`, nao ha caso de uso. Aplicar MediatR a
um botao seria cerimonia sem mensagem para carregar.

## O que continua valendo do padrao

.NET 10, cobertura ≥ 80%, fluxo de branch/PR, cultura explicita em formatacao, texto ao usuario
final em PT-BR, segredos fora do repositorio, CI/CD no GitHub Actions.

## Consequencia

Uma sessao futura que abrir este repositorio vai estranhar a ausencia de `Features/`. Este ADR
existe para que ela **nao "corrija" a arquitetura para VSA** — a divergencia e deliberada.
