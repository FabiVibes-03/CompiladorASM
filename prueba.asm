;Archivo: prueba.cpp
;Fecha y hora: 20/03/2025 10:44:46 p. m.
;----------------------------------
%include "io.inc"
segment .text
global main
main:
; Asignacion de c
	MOV EAX, 100
	PUSH EAX
	MOV EAX, 200
	PUSH EAX
	POP EAX
	POP EBX
	ADD EAX, EBX
	PUSH EAX
	POP EAX
	MOV BYTE [c], AL
PRINT_STRING msg_1
NEWLINE
GET_DEC 4, altura
; Asignacion de x
	MOV EAX, 3
	PUSH EAX
	MOV EAX, [altura]
	PUSH EAX
	POP EAX
	POP EBX
	ADD EAX, EBX
	PUSH EAX
	MOV EAX, 8
	PUSH EAX
	POP EBX
	POP EAX
	MUL EBX
	PUSH EAX
	MOV EAX, 10
	PUSH EAX
	MOV EAX, 4
	PUSH EAX
	POP EAX
	POP EBX
	SUB EAX, EBX
	PUSH EAX
	MOV EAX, 2
	PUSH EAX
	POP EBX
	POP EAX
	XOR EDX, EDX
	DIV EBX
	PUSH EAX
	POP EAX
	POP EBX
	SUB EAX, EBX
	PUSH EAX
	POP EAX
	MOV DWORD [x], EAX
; Decremento variable: x
	MOV AX, [x]
	SUB AX, 1
	MOV [x], AX
	MOV EAX, [altura]
	PUSH EAX
	MOV EAX, 8
	PUSH EAX
	POP EBX
	POP EAX
	MUL EBX
	PUSH EAX
; x += Expresion
	POP EAX
	ADD [x], EAX
	MOV EAX, 2
	PUSH EAX
; x *= Expresion
	POP EBX
	MOV EAX, [x]
	MUL EBX
	MOV [x], EAX
	MOV EAX, [y]
	PUSH EAX
	MOV EAX, 6
	PUSH EAX
	POP EAX
	POP EBX
	SUB EAX, EBX
	PUSH EAX
; x /= Expresion
	POP EBX
	MOV EAX, [x]
	XOR EDX, EDX
	DIV EBX
	MOV [x], EAX
; Asignacion de i
	MOV EAX, 1
	PUSH EAX
	POP EAX
	MOV WORD [i], AX
for_1:
	MOV EAX, [i]
	PUSH EAX
	MOV EAX, [altura]
	PUSH EAX
	POP EBX
	POP EAX
	CMP EAX, EBX
	JG end_for_1
; Asignacion de j
	MOV EAX, 1
	PUSH EAX
	POP EAX
	MOV WORD [j], AX
for_2:
	MOV EAX, [j]
	PUSH EAX
	MOV EAX, [i]
	PUSH EAX
	POP EBX
	POP EAX
	CMP EAX, EBX
	JG end_for_2
; if (condicion) 
	MOV EAX, [j]
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
PRINT_STRING msg_2
	jmp jump_end_if_1
jump_if_else_1:
PRINT_STRING msg_3
jump_end_if_1:
; Incremento variable: j usando ADD
	mov eax, [j]
	add eax, 1
	mov [j], eax
	jmp for_2
end_for_2:
NEWLINE
; Incremento variable: i usando ADD
	mov eax, [i]
	add eax, 1
	mov [i], eax
	jmp for_1
end_for_1:
; Asignacion de i
	MOV EAX, 0
	PUSH EAX
	POP EAX
	MOV WORD [i], AX
;DoWhile (condicion) 
jump_do_1:
PRINT_STRING msg_4
; Incremento variable: i
	MOV AX, [i]
	ADD AX, 1
	MOV [i], AX
	MOV EAX, [i]
	PUSH EAX
	MOV EAX, [altura]
	PUSH EAX
	MOV EAX, 2
	PUSH EAX
	POP EBX
	POP EAX
	MUL EBX
	PUSH EAX
	POP EBX
	POP EAX
	CMP EAX, EBX
	JL jump_do_1
NEWLINE
; Asignacion de i
	MOV EAX, 1
	PUSH EAX
	POP EAX
	MOV WORD [i], AX
for_3:
	MOV EAX, [i]
	PUSH EAX
	MOV EAX, [altura]
	PUSH EAX
	POP EBX
	POP EAX
	CMP EAX, EBX
	JG end_for_3
; Asignacion de j
	MOV EAX, 1
	PUSH EAX
	POP EAX
	MOV WORD [j], AX
while_1:
	MOV EAX, [j]
	PUSH EAX
	MOV EAX, [i]
	PUSH EAX
	POP EBX
	POP EAX
	CMP EAX, EBX
	JG end_while_1
PRINT_DEC 4, j
; Incremento variable: j
	MOV AX, [j]
	ADD AX, 1
	MOV [j], AX
	JMP while_1
end_while_1:
NEWLINE
; Incremento variable: i usando ADD
	mov eax, [i]
	add eax, 1
	mov [i], eax
	jmp for_3
end_for_3:
; Asignacion de i
	MOV EAX, 0
	PUSH EAX
	POP EAX
	MOV WORD [i], AX
;DoWhile (condicion) 
jump_do_2:
PRINT_STRING msg_5
; Incremento variable: i
	MOV AX, [i]
	ADD AX, 1
	MOV [i], AX
	MOV EAX, [i]
	PUSH EAX
	MOV EAX, [altura]
	PUSH EAX
	MOV EAX, 2
	PUSH EAX
	POP EBX
	POP EAX
	MUL EBX
	PUSH EAX
	POP EBX
	POP EAX
	CMP EAX, EBX
	JL jump_do_2
NEWLINE
	RET
section .data
altura DD 0
i DD 0
j DD 0
y DD 0
x DW 0.0
c DB 0
msg_1 DD "Valor de altura = ", 0
msg_2 DD "*", 0
msg_3 DD "-", 0
msg_4 DD "-", 0
msg_5 DD "-", 0
