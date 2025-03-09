;Archivo: prueba.cpp
;Fecha y hora: 09/03/2025 05:49:43 p. m.
;----------------------------------
segment .text
global main
main:
	MOV EAX, 0
	PUSH EAX 6
	POP EAX 1
	MOV [x26], EAX
while_1:
	MOV EAX, [x26]
	PUSH EAX
	MOV EAX, 10
	PUSH EAX 6
	POP EBX 7
	POP EAX 8
	CMP EAX, EBX
	JGE end_while_1
	MOV EAX, 1
	PUSH EAX 6
; x26 += Expresion
	POP EAX
	ADD [x26], EAX
	JMP while_1
end_while_1:
	RET
section .data
x26 DW 0
