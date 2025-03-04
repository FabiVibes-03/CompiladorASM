;Archivo: prueba.cpp
;Fecha y hora: 04/03/2025 11:53:39 a. m.
;----------------------------------
segment .text
global main
main:
	MOV EAX, 200
	PUSH EAX
	POP EAX 1
	MOV [x26], EAX
;DoWhile (condicion) 
jump_do_1:
; Asignacion de x26
	MOV EAX, [x26]
	PUSH EAX
	MOV EAX, 1
	PUSH EAX
	POP EAX 9
	POP EBX 10
	ADD EAX, EBX
	PUSH EBX
	POP EAX 1.2
	MOV [x26], EAX
	MOV EAX, [x26]
	PUSH EAX
	POP EBX 7
	MOV EAX, 211
	PUSH EAX
	POP EAX 8
	CMP EAX, EBX
	JB jump_do_1
	RET
section .data
x26 DW 0
