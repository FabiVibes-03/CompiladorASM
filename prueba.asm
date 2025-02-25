;Archivo: prueba.cpp
;Fecha y hora: 25/02/2025 11:51:56 a. m.
;----------------------------------
SEGMENT .TEXT
GLOBAL MAIN
MAIN:
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
     MUL EBX, EAX
     PUSH AX
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
     DIV EBX, EAX
     PUSH AL
    POP EAX
    POP EBX
     SUB EBX, EAX
     PUSH EBX
    POP
SECTION .DATA
x26 DW ?
