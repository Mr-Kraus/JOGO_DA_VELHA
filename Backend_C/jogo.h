#ifndef JOGO_H
#define JOGO_H

#define EXPORT __declspec(dllexport)

#ifdef __cplusplus
extern "C" {
#endif

EXPORT void iniciar_jogo();
EXPORT void finalizar_jogo();
EXPORT int preenche_pos(int jogador, int linha, int coluna);
EXPORT int get_posicao(int linha, int coluna);
EXPORT int verificar_vitoria();

#ifdef __cplusplus
}
#endif

#endif