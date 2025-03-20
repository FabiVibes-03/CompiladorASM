;Archivo: prueba.cpp
;Fecha y hora: 20/03/2025 12:03:47 p. m.
;----------------------------------
%include "io.inc"
segment .text
global main
main:
	MOV EAX, 7
	PUSH EAX
	POP EAX
	MOV [y], EAX
mov eax, buffer
call getchar  ; Leer un solo carácter
; Leyendo: 3
	RET
section .data
altura DD 0
i DD 0
j DD 0
x DD 0
y DD 0
