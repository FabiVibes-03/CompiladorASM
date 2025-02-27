;Archivo: prueba.cpp
;Fecha y hora: 27/02/2025 11:13:59 a. m.
;----------------------------------
SEGMENT .TEXT
GLOBAL main
main:
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
SECTION .DATA
x26 DW 0
