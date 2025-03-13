;Archivo: prueba.cpp
;Fecha y hora: 13/03/2025 11:49:11 a. m.
;----------------------------------
segment .text
global main
main:
; Asignacion de sum
	MOV EAX, 0
	PUSH EAX
	POP EAX
	MOV WORD [sum], AX
; Asignacion de i
	MOV EAX, 0
	PUSH EAX
	POP EAX
	MOV WORD [i], AX
for_1:
	MOV EAX, [i]
	PUSH EAX
	MOV EAX, 5
	PUSH EAX
	POP EBX
	POP EAX
	CMP EAX, EBX
	JGE end_for_1
; if (condicion) 
	MOV EAX, [i]
	PUSH EAX
	MOV EAX, 2
	PUSH EAX
	POP EBX
	POP EAX
	XOR EDX, EDX
	DIV EBX
	PUSH EDX
	MOV EAX, 0
	PUSH EAX
	POP EBX
	POP EAX
	CMP EAX, EBX
	JNE jump_if_else_1
	MOV EAX, [i]
	PUSH EAX
; sum += Expresion
	POP EAX
	ADD [sum], EAX
	jmp jump_end_if_1
jump_if_else_1:
	MOV EAX, [i]
	PUSH EAX
	MOV EAX, 2
	PUSH EAX
	POP EBX
	POP EAX
	MUL EBX
	PUSH EAX
; sum += Expresion
	POP EAX
	ADD [sum], EAX
jump_end_if_1:
; Incremento variable: i usando ADD
	mov eax, [i]
	add eax, 1
	mov [i], eax
	jmp for_1
end_for_1:
; Asignacion de j
	MOV EAX, 0
	PUSH EAX
	POP EAX
	MOV WORD [j], AX
while_1:
	MOV EAX, [j]
	PUSH EAX
	MOV EAX, 5
	PUSH EAX
	POP EBX
	POP EAX
	CMP EAX, EBX
	JGE end_while_1
; Asignacion de sum
	MOV EAX, [sum]
	PUSH EAX
	MOV EAX, [j]
	PUSH EAX
	MOV EAX, 2
	PUSH EAX
	POP EBX
	POP EAX
	MUL EBX
	PUSH EAX
	POP EAX
	POP EBX
	ADD EAX, EBX
	PUSH EAX
	POP EAX
	MOV WORD [sum], AX
; Incremento variable: j
	MOV AX, [j]
	ADD AX, 1
	MOV [j], AX
	JMP while_1
end_while_1:
; Asignacion de j
	MOV EAX, 0
	PUSH EAX
	POP EAX
	MOV WORD [j], AX
;DoWhile (condicion) 
jump_do_1:
; Asignacion de sum
	MOV EAX, [sum]
	PUSH EAX
	MOV EAX, [j]
	PUSH EAX
	MOV EAX, 3
	PUSH EAX
	POP EBX
	POP EAX
	MUL EBX
	PUSH EAX
	POP EAX
	POP EBX
	ADD EAX, EBX
	PUSH EAX
	POP EAX
	MOV WORD [sum], AX
; Incremento variable: j
	MOV AX, [j]
	ADD AX, 1
	MOV [j], AX
	MOV EAX, [j]
	PUSH EAX
	MOV EAX, 5
	PUSH EAX
	POP EBX
	POP EAX
	CMP EAX, EBX
	JL jump_do_1
	RET
section .data
sum DD 0
i DD 0
j DD 0
