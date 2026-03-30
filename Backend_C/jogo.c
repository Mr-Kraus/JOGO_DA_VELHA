#include "jogo.h"
#include <stdlib.h>

typedef enum {
    VAZIO = 0,
    JOGADOR_X = 1,
    JOGADOR_O = 2
} ValorCasa;

typedef struct {
    ValorCasa *tabuleiro;
    int jogadas_realizadas;
} EstadoJogo;

static EstadoJogo *jogo_atual = NULL;

EXPORT void iniciar_jogo() {
    if (jogo_atual != NULL) {
        finalizar_jogo();
    }

    jogo_atual = (EstadoJogo*) malloc(sizeof(EstadoJogo));
    jogo_atual->tabuleiro = (ValorCasa*) malloc(9 * sizeof(ValorCasa));
    jogo_atual->jogadas_realizadas = 0;

    for (int i = 0; i < 9; i++) {
        *(jogo_atual->tabuleiro + i) = VAZIO; 
    }
}

EXPORT void finalizar_jogo() {
    if (jogo_atual != NULL) {
        free(jogo_atual->tabuleiro);
        free(jogo_atual);
        jogo_atual = NULL;
    }
}

EXPORT int preenche_pos(int jogador, int linha, int coluna) {
    // Validação de limites para garantir que o aluno não acesse memória inválida
    if (jogo_atual == NULL || linha < 0 || linha > 2 || coluna < 0 || coluna > 2) return -1;
    
    // Fórmula para mapear coordenadas 2D (linha, coluna) para array 1D
    int indice = (linha * 3) + coluna;
    ValorCasa *casa_alvo = &jogo_atual->tabuleiro[indice];
    
    if (*casa_alvo == VAZIO) {
        *casa_alvo = (ValorCasa)jogador;
        jogo_atual->jogadas_realizadas++;
        return 0; // Sucesso
    }
    return -1; // Posição já ocupada
}

EXPORT int get_posicao(int linha, int coluna) {
    if (jogo_atual == NULL || linha < 0 || linha > 2 || coluna < 0 || coluna > 2) return VAZIO;
    return jogo_atual->tabuleiro[(linha * 3) + coluna];
}

EXPORT int verificar_vitoria() {
    if (jogo_atual == NULL) return VAZIO;

    int vitorias[8][3] = {
        {0, 1, 2}, {3, 4, 5}, {6, 7, 8}, // Linhas
        {0, 3, 6}, {1, 4, 7}, {2, 5, 8}, // Colunas
        {0, 4, 8}, {2, 4, 6}             // Diagonais
    };

    for (int i = 0; i < 8; i++) {
        ValorCasa val_a = jogo_atual->tabuleiro[vitorias[i][0]];
        ValorCasa val_b = jogo_atual->tabuleiro[vitorias[i][1]];
        ValorCasa val_c = jogo_atual->tabuleiro[vitorias[i][2]];

        if (val_a != VAZIO && val_a == val_b && val_a == val_c) {
            return val_a; // 1 ou 2 (Alguém venceu)
        }
    }
    
    // Se chegou a 9 jogadas e ninguém venceu, é empate
    if (jogo_atual->jogadas_realizadas == 9) {
        return 3; // Código 3 para empate
    }
    
    return VAZIO; // Jogo continua
}