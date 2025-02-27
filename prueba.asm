;Archivo: prueba.cpp
<<<<<<< HEAD
;Fecha y hora: 27/02/2025 11:16:11 a. m.
=======
;Fecha y hora: 27/02/2025 11:13:59 a. m.
>>>>>>> 6a2af169c5eb637c2fc624efefef4bc2aba9308f
;----------------------------------
SEGMENT .TEXT
GLOBAL main
main:
     MOV EAX, 0
     PUSH EAX
     MOV EAX, 3
     PUSH EAX
     MOV EAX, 5
     PUSH EAX
    POP EAX
    POP EBX
     ADD EBX, EAX
     PUSH EBX
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
     SUB EBX, EAX
     PUSH EBX
     MOV EAX, 2
     PUSH EAX
    POP EBX
    POP EAX
    XOR EDX, EDX
     DIV EBX
     PUSH EAX
    POP EAX
    POP EBX
     SUB EBX, EAX
     PUSH EBX
    POP 7
     MOV EAX, 61
     PUSH EAX
    POP 8
; Asignacion de x26
     MOV EAX, 100
     PUSH EAX
    POP EAX
    MOV DWORD[x26], EAX
SECTION .DATA
x26 DW 0
