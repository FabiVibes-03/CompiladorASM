;Archivo: prueba.cpp
;Fecha y hora: 03/03/2025 11:06:43 a. m.
;----------------------------------
SEGMENT .TEXT
GLOBAL main
main:
	MOV EAX, 200
	PUSH EAX 6
;DoWhile (condicion) 
jump_do_1:
	POP EAX
	MOV [x26], EAX
; Asignacion de x26
	MOV EAX, x26
	PUSH EAX
	MOV EAX, 1
	PUSH EAX 6
	POP EAX
	POP EBX
	ADD EAX, EBX
	PUSH EBX
	POP EAX
	MOV x26, EAX
	MOV EAX, x26
	PUSH EAX
	POP EBX
	MOV EAX, 211
	PUSH EAX 6
	POP EAX
	CMP EAX, EBX
	JB jump_do_1
	RET
SECTION .DATA
x26 DW 0
