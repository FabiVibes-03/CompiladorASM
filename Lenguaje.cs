/*
    //SECTION REQUERIMIENTOS:
    1.- Declarar las variables en ensamblador con su tipo de dato (Listo)
    2.- En la asignacion generar codigo en ensamblador para ++(inc) --(dec) (Listo)
    3.- Para +=, -= (Listo)
    4.- Generar codigo ensamblador para Console.Write y Console.WriteLine
    5.- Generar codigo ensamblador para Console.Read y Console.ReadLine
    6.- Programar el While (Listo)
    7.- Programar el For (Listo)
    8.- Programar else (Listo)
    9.- Usar set y get en variables //NOTE - Volverlo a hacer en variable
    10.- Ajustar todos los parametros por default 
  //!SECTION  
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace ASM
{
    public class Lenguaje : Sintaxis
    {

        //@params GLOBALES

        private int ifCounter, whileCounter, doWhileCounter, forCounter, msgCounter;
        public bool isConsoleRead;
        Stack<float> s;
        List<Variable> l;
        Variable.TipoDato maximoTipo;
        bool huboCasteo = false;
        Variable.TipoDato tipoCasteo = Variable.TipoDato.Char;
        //!@params


        //SECTION - CONSTRUCTORES
        public Lenguaje() : base()
        {
            s = new Stack<float>();
            l = new List<Variable>();
            log.WriteLine("Constructor lenguaje");
            maximoTipo = Variable.TipoDato.Char;
            ifCounter = whileCounter = doWhileCounter = forCounter = msgCounter = 1;
            isConsoleRead = false;
        }
        public Lenguaje(string nombre) : base(nombre)
        {
            s = new Stack<float>();
            l = new List<Variable>();
            log.WriteLine("Constructor lenguaje");
            maximoTipo = Variable.TipoDato.Char;
            ifCounter = whileCounter = doWhileCounter = forCounter = msgCounter = 1;
            isConsoleRead = false;
        }
        //!SECTION

        //SECTION - displayStack
        private void displayStack()
        {
            Console.WriteLine("Contenido del stack: ");
            foreach (float elemento in s)
            {
                Console.WriteLine(elemento);
            }
        }
        //!SECTION

        //SECTION - displayLista
        private void displayLista()
        {
            asm.WriteLine("section .data");
            log.WriteLine("Lista de variables: ");
            foreach (Variable elemento in l)
            {
                switch (elemento.Tipo)
                {
                    //NOTE - Requerimiento 1
                    case Variable.TipoDato.Char:
                        log.WriteLine($"{elemento.Nombre} {elemento.Tipo} {elemento.Valor}");
                        asm.WriteLine($"{elemento.Nombre} DB 0");
                        break;
                    case Variable.TipoDato.Int:
                        log.WriteLine($"{elemento.Nombre} {elemento.Tipo} {elemento.Valor}");
                        asm.WriteLine($"{elemento.Nombre} DD 0");
                        break;
                    case Variable.TipoDato.Float:
                        log.WriteLine($"{elemento.Nombre} {elemento.Tipo} {elemento.Valor}");
                        asm.WriteLine($"{elemento.Nombre} DD 0.0");
                        break;
                }
            }
        }
        //!SECTION

        //SECTION - Programa
        //Programa  -> Librerias? Variables? Main
        public void Programa()
        {
            if (Contenido == "using")
            {
                Librerias();
            }
            if (Clasificacion == Tipos.TipoDato)
            {
                Variables();
            }
            Main();
            asm.WriteLine("\tRET");
            displayLista();
        }
        //Librerias -> using ListaLibrerias; Librerias?
        //!SECTION

        //SECTION - Librerias
        private void Librerias()
        {
            match("using");
            ListaLibrerias();
            match(";");
            if (Contenido == "using")
            {
                Librerias();
            }
        }
        //Variables -> tipo_dato Lista_identificadores; Variables?
        //!SECTION

        //SECTION - Variables
        private void Variables()
        {
            Variable.TipoDato t = Variable.TipoDato.Char;
            switch (Contenido)
            {
                case "int": t = Variable.TipoDato.Int; break;
                case "float": t = Variable.TipoDato.Float; break;
            }
            match(Tipos.TipoDato);
            ListaIdentificadores(t);
            match(";");
            if (Clasificacion == Tipos.TipoDato)
            {
                Variables();
            }
        }
        //!SECTION
        //SECTION - ListaLibrerias
        //ListaLibrerias -> identificador (.ListaLibrerias)?
        private void ListaLibrerias()
        {
            match(Tipos.Identificador);
            if (Contenido == ".")
            {
                match(".");
                ListaLibrerias();
            }
        }
        //!SECTION
        //SECTION - ListaIdentificadores
        //ListaIdentificadores -> identificador (= Expresion)? (,ListaIdentificadores)?
        private void ListaIdentificadores(Variable.TipoDato t)
        {
            if (l.Find(variable => variable.Nombre == Contenido) != null)
            {
                throw new Error($"La variable {Contenido} ya existe", log, linea, columna);
            }

            Variable v = new Variable(t, Contenido);
            l.Add(v);

            match(Tipos.Identificador);
            if (Contenido == "=")
            {
                match("=");
                if (Contenido == "Console")
                {
                    match("Console");
                    match(".");
                    if (Contenido == "Read")
                    {
                        match("Read");
                        int r = Console.Read();
                        v.setValor(r, linea, columna, log, maximoTipo); // Asignamos el último valor leído a la última variable detectada
                    }
                    else
                    {
                        match("ReadLine");
                        string? r = Console.ReadLine();
                        float result;

                        if (float.TryParse(r, out result))
                        {
                            v.setValor(result, linea, columna, log, maximoTipo);
                        }
                        else
                        {
                            throw new Error("Sintaxis. No se ingresó un número ", log, linea, columna);
                        }
                    }
                    match("(");
                    match(")");
                }
                else
                {
                    // Como no se ingresó un número desde el Console, entonces viene de una expresión matemática
                    Expresion();
                    float resultado = s.Pop();
                    //NOTE - REVISAR ESTE POP
                    asm.WriteLine("\tPOP EAX");
                    asm.WriteLine($"\tMOV [{v.Nombre}], EAX");
                    v.setValor(resultado, linea, columna, log, maximoTipo);
                }
            }
            if (Contenido == ",")
            {
                match(",");
                ListaIdentificadores(t);
            }
        }
        //!SECTION
        //SECTION - BloqueInstrucciones
        //BloqueInstrucciones -> { listaIntrucciones? }
        private void BloqueInstrucciones(bool ejecuta)
        {
            match("{");
            if (Contenido != "}")
            {
                ListaInstrucciones(ejecuta);
            }
            else
            {
                match("}");
            }
        }
        //!SECTION
        //SECTION - ListaInstrucciones
        //ListaInstrucciones -> Instruccion ListaInstrucciones?
        private void ListaInstrucciones(bool ejecuta)
        {
            Instruccion(ejecuta);
            if (Contenido != "}")
            {
                ListaInstrucciones(ejecuta);
            }
            else
            {
                match("}");
            }
        }
        //!SECTION
        //SECTION - Instruccion
        //Instruccion -> console | If | While | do | For | Variables | Asignación
        private void Instruccion(bool execute)
        {
            switch (Contenido)
            {
                case "Console":
                    console(execute);
                    break;
                case "if":
                    If(execute);
                    break;
                case "while":
                    While(execute);
                    break;
                case "do":
                    Do(execute);
                    break;
                case "for":
                    For(execute);
                    break;
                default:
                    if (Clasificacion == Tipos.TipoDato)
                    {
                        Variables();
                    }
                    else
                    {
                        Asignacion(execute);
                        match(";");
                    }
                    break;
            }
        }
        //!SECTION
        //Asignacion -> Identificador = Expresion; (DONE)
        /*
        Id++ (DONE)
        Id-- (DONE)
        Id IncrementoTermino Expresion (DONE)
        Id IncrementoFactor Expresion (DONE)
        Id = Console.Read() (DONE)
        Id = Console.ReadLine() (DONE)
        */

        //SECTION - Asignacion
        private void Asignacion(bool execute)
        {
            huboCasteo = false;
            tipoCasteo = Variable.TipoDato.Char;
            maximoTipo = Variable.TipoDato.Char;

            float r;
            Variable? v = l.Find(variable => variable.Nombre == Contenido);
            if (v == null)
            {
                throw new Error($"Sintaxis: La variable {Contenido} no está definida", log, linea, columna);
            }
            //Console.Write(Contenido + " = ");
            match(Tipos.Identificador);
            //NOTE - Requerimeinto 2
            switch (Contenido)
            {
                case "++":
                    match("++");
                    asm.WriteLine($"; Incremento variable: {v.Nombre}");
                    asm.WriteLine($"\tMOV AX, [{v.Nombre}]"); // Carga el valor en AX
                    asm.WriteLine($"\tADD AX, 1"); // Suma 1
                    asm.WriteLine($"\tMOV [{v.Nombre}], AX");
                    r = v.getValor() + 1;
                    v.setValor(r, linea, columna, log, maximoTipo);
                    break;
                case "--":
                    match("--");
                    asm.WriteLine($"; Decremento variable: {v.Nombre}");
                    asm.WriteLine($"\tMOV AX, [{v.Nombre}]"); // Carga el valor en AX
                    asm.WriteLine($"\tSUB AX, 1"); // Resta 1
                    asm.WriteLine($"\tMOV [{v.Nombre}], AX");
                    r = v.getValor() - 1;
                    v.setValor(r, linea, columna, log, maximoTipo);
                    break;
                case "=":
                    match("=");
                    if (Contenido == "Console")
                    {
                        ListaIdentificadores(v.Tipo); // Ya se hace este procedimiento arriba así que simplemente obtenemos a través del método lo que necesitamos
                    }
                    else
                    {
                        asm.WriteLine($"; Asignacion de {v.Nombre}");
                        Expresion();
                        r = s.Pop();
                        //REVIEW - Depende del tipo de dato se usa cierta instruccion
                        asm.WriteLine("\tPOP EAX");

                        switch (v.Tipo)
                        {
                            case Variable.TipoDato.Char:
                                asm.WriteLine($"\tMOV BYTE [{v.Nombre}], AL");
                                break;
                            case Variable.TipoDato.Int:
                                asm.WriteLine($"\tMOV WORD [{v.Nombre}], AX"); //REVIEW - Checar este AX
                                break;
                            case Variable.TipoDato.Float:
                                asm.WriteLine($"\tMOV DWORD [{v.Nombre}], EAX");
                                break;
                        }
                        maximoTipo = Variable.valorTipoDato(r, maximoTipo, huboCasteo);
                        v.setValor(r, linea, columna, log, maximoTipo);
                    }
                    break;
                //NOTE - Requerimiento 3
                case "+=":
                    match("+=");
                    Expresion();
                    r = v.getValor() + s.Pop();
                    asm.WriteLine($"; {v.Nombre} += Expresion");
                    asm.WriteLine("\tPOP EAX");
                    asm.WriteLine($"\tADD [{v.Nombre}], EAX"); // Sumar directamente en memoria
                    maximoTipo = Variable.valorTipoDato(r, maximoTipo, huboCasteo);
                    v.setValor(r, linea, columna, log, maximoTipo);
                    break;
                case "-=":
                    match("-=");
                    Expresion();
                    r = v.getValor() - s.Pop();
                    asm.WriteLine($"; {v.Nombre} -= Expresion");
                    asm.WriteLine("\tPOP EAX");
                    asm.WriteLine($"\tSUB [{v.Nombre}], EAX"); // Restar directamente en memoria
                    maximoTipo = Variable.valorTipoDato(r, maximoTipo, huboCasteo);
                    v.setValor(r, linea, columna, log, maximoTipo);
                    break;
                case "*=":
                    match("*=");
                    Expresion();
                    r = v.getValor() * s.Pop();
                    asm.WriteLine($"; {v.Nombre} *= Expresion");
                    asm.WriteLine("\tPOP EBX");
                    asm.WriteLine($"\tMOV EAX, [{v.Nombre}]");
                    asm.WriteLine("\tMUL EBX");  // Multiplicacion
                    asm.WriteLine($"\tMOV [{v.Nombre}], EAX");
                    maximoTipo = Variable.valorTipoDato(r, maximoTipo, huboCasteo);
                    v.setValor(r, linea, columna, log, maximoTipo);
                    break;
                case "/=":
                    match("/=");
                    Expresion();
                    r = v.getValor() / s.Pop();
                    asm.WriteLine($"; {v.Nombre} /= Expresion");

                    asm.WriteLine("\tPOP EBX");
                    asm.WriteLine("\tMOV EAX, [{v.getNombre()}]");
                    asm.WriteLine("\tXOR EDX, EDX"); // Limpiar registros
                    asm.WriteLine("\tDIV EBX");  // Division -> resultado = EAX y residuo = EDX
                    asm.WriteLine($"\tMOV [{v.Nombre}], EAX");
                    maximoTipo = Variable.valorTipoDato(r, maximoTipo, huboCasteo);
                    v.setValor(r, linea, columna, log, maximoTipo);
                    break;
                case "%=":
                    match("%=");
                    Expresion();
                    r = v.getValor() % s.Pop();
                    asm.WriteLine($"; {v.Nombre} %= Expresion");
                    asm.WriteLine("\tPOP EBX");
                    asm.WriteLine("\tMOV EAX, [{v.getNombre()}]");
                    asm.WriteLine("\tXOR EDX, EDX"); //Limpiar registros
                    asm.WriteLine("\tDIV EBX");  // Division -> resultado = EAX y residuo = EDX
                    asm.WriteLine($"\tMOV [{v.Nombre}], EDX");
                    maximoTipo = Variable.valorTipoDato(r, maximoTipo, huboCasteo);
                    v.setValor(r, linea, columna, log, maximoTipo);
                    break;
            }
            //displayStack();
        }

        //!SECTION

        //SECTION - IF
        /*If -> if (Condicion) bloqueInstrucciones | instruccion
        (else bloqueInstrucciones | instruccion)?*/
        private void If(bool execute2)
        {
            match("if");
            match("(");

            asm.WriteLine("; if (condicion) ");
            string label = $"jump_if_else_{ifCounter}";
            string labelEndIf = $"jump_end_if_{ifCounter}";
            ifCounter++;

            bool execute = Condicion(label) && execute2;
            match(")");
            if (Contenido == "{")
            {
                BloqueInstrucciones(execute);
            }
            else
            {
                Instruccion(execute);
            }

            asm.WriteLine($"\tjmp {labelEndIf}");
            asm.WriteLine($"{label}:");

            if (Contenido == "else")
            {
                match("else");
                bool executeElse = !execute; // Solo se ejecuta el else si el if no se ejecutó
                if (Contenido == "{")
                {
                    BloqueInstrucciones(executeElse);
                }
                else
                {
                    Instruccion(executeElse);
                }
            }
            asm.WriteLine($"{labelEndIf}:");
        }
        //!SECTION
        //SECTION - Condicion
        //Condicion -> Expresion operadorRelacional Expresion
        private bool Condicion(string label, bool isDo = false)
        {
            maximoTipo = Variable.TipoDato.Char;
            Expresion();
            float valor1 = s.Pop();

            string operador = Contenido;
            match(Tipos.OperadorRelacional);

            maximoTipo = Variable.TipoDato.Char;

            Expresion();
            float valor2 = s.Pop();
            asm.WriteLine("\tPOP EBX");
            asm.WriteLine("\tPOP EAX");
            asm.WriteLine("\tCMP EAX, EBX");

            if (!isDo)
            {
                switch (operador)
                {
                    //REVIEW - CAMBIE JA POR JB, YA QUE JB COMPARA SIGNOS Y JA NO
                    case ">":
                        asm.WriteLine($"\tJLE {label}");
                        return valor1 > valor2;
                    case ">=":
                        asm.WriteLine($"\tJL {label}");
                        return valor1 >= valor2;
                    case "<":
                        asm.WriteLine($"\tJGE {label}");
                        return valor1 < valor2;
                    case "<=":
                        asm.WriteLine($"\tJG {label}");
                        return valor1 <= valor2;
                    case "==":
                        asm.WriteLine($"\tJNE {label}");
                        return valor1 == valor2;
                    default:
                        asm.WriteLine($"\tJE {label}");
                        return valor1 != valor2;
                }
            }
            else
            {
                switch (operador)
                {
                    case ">":
                        asm.WriteLine($"\tJG {label}");
                        return valor1 > valor2;
                    case ">=":
                        asm.WriteLine($"\tJGE {label}");
                        return valor1 >= valor2;
                    case "<":
                        asm.WriteLine($"\tJL {label}");
                        return valor1 < valor2;
                    case "<=":
                        asm.WriteLine($"\tJLE {label}");
                        return valor1 <= valor2;
                    case "==":
                        asm.WriteLine($"\tJE {label}");
                        return valor1 == valor2;
                    default:
                        asm.WriteLine($"\tJNE {label}");
                        return valor1 != valor2;
                }
            }

        }
        //!SECTION
        //SECTION - While
        //While -> while(Condicion) bloqueInstrucciones | instruccion
        private void While(bool execute)
        {
            match("while");
            match("(");

            string inicioWhile = $"while_{whileCounter}";
            string finWhile = $"end_while_{whileCounter}";
            whileCounter++;
            asm.WriteLine($"{inicioWhile}:");
            bool condicionValida = Condicion(finWhile);
            match(")");
            if (Contenido == "{")
            {
                BloqueInstrucciones(condicionValida);
            }
            else
            {
                Instruccion(condicionValida);
            }

            asm.WriteLine($"\tJMP {inicioWhile}"); // Regresa al inicio del while
            asm.WriteLine($"{finWhile}:");
        }
        //!SECTION
        //SECTION - DO
        /*Do -> do bloqueInstrucciones | intruccion 
        while(Condicion);*/
        private void Do(bool execute)
        {
            asm.WriteLine(";DoWhile (condicion) ");
            string label = $"jump_do_{doWhileCounter++}";
            //NOTE - No me acuerdo que hace aqui
            asm.WriteLine($"{label}:");

            match("do");
            if (Contenido == "{")
            {
                BloqueInstrucciones(execute);
            }
            else
            {
                Instruccion(execute);
            }

            match("while");
            match("(");
            Condicion(label, true);
            match(")");
            match(";");
        }
        //!SECTION
        //SECTION - For
        /*For -> for(Asignacion; Condicion; Asignacion) 
        BloqueInstrucciones | Intruccion*/
        private void For(bool execute)
        {
            match("for");
            match("(");

            Asignacion(execute);
            match(";");

            string etiquetaInicio = $"for_{forCounter}";
            string etiquetaFin = $"end_for_{forCounter}";
            forCounter++;

            asm.WriteLine($"{etiquetaInicio}:");

            bool condicionValida = Condicion(etiquetaFin);
            match(";");

            string[] tokensIncremento = CapturarIncrementoTokens();
            match(")");

            if (Contenido == "{")
            {
                BloqueInstrucciones(condicionValida);
            }
            else
            {
                Instruccion(condicionValida);
            }

            ProcesarIncremento(execute, tokensIncremento);
            asm.WriteLine($"\tjmp {etiquetaInicio}");
            asm.WriteLine($"{etiquetaFin}:");
        }
        //!SECTION
        //SECTION - Console
        //Console -> Console.(WriteLine|Write) (cadena? concatenaciones?);
        private void console(bool execute)
        {
            match("Console");
            match(".");

            bool saltoLinea = false;
            isConsoleRead = false;


            switch (Contenido)
            {
                case "WriteLine":
                    saltoLinea = true;
                    match("WriteLine");
                    break;
                case "Write":
                    match("Write");
                    break;
                case "ReadLine":
                    isConsoleRead = true;
                    match("ReadLine");
                    break;
                case "Read":
                    isConsoleRead = true;
                    match("Read");
                    break;
                default:
                    throw new Error("Sintaxis: Se esperaba Write o WriteLine", log, linea, columna);
            }

            match("(");

            if (!isConsoleRead)
            {
                if (Clasificacion == Tipos.Cadena)
            {
                string label = $"msg_{msgCounter++}";
                asm.WriteLine($"; Imprimiendo cadena: {Contenido}");

                // Se define la cadena en la sección de datos
                asm.WriteLine("section .data");
                string literal = Contenido.Replace("\"", "");
                asm.WriteLine($"{label} db \"{literal}\", 0");

                // Se vuelve a la sección de texto y se invoca la macro PRINT_STRING
                asm.WriteLine("segment .text");
                asm.WriteLine($"\tPRINT_STRING {label}");
                match(Tipos.Cadena);
            }
            else if (Clasificacion == Tipos.Identificador)
            {
                Variable? v = l.Find(variable => variable.Nombre == Contenido);
                if (v == null)
                {
                    throw new Error("La variable no existe", log, linea, columna);
                }

                asm.WriteLine($"; Imprimiendo variable: {v.Nombre}");
                asm.WriteLine($"\tMOV EAX, [{v.Nombre}]");
                asm.WriteLine("\tPRINT_UDEC 4, EAX"); // Usar macro PRINT_DEC para imprimir números

                match(Tipos.Identificador);
            }
            }
            

            match(")");
            match(";");

            if (!isConsoleRead)
            {
                if (saltoLinea)
                {
                    asm.WriteLine("\tNEWLINE");
                }
            }
            else
            {
                // Para lectura, se genera la instrucción GET_DEC para leer un entero de 4 bytes
                asm.WriteLine("\tGET_DEC 4, eax");
            }
        }

        //!SECTION
        //SECTION - Concatenaciones
        // Concatenaciones -> Identificador|Cadena ( + concatenaciones )?
        private string Concatenaciones()
        {
            string resultado = "";
            if (Clasificacion == Tipos.Identificador)
            {
                Variable? v = l.Find(variable => variable.Nombre == Contenido);
                if (v != null)
                {
                    resultado = v.getValor().ToString(); // Obtener el valor de la variable y convertirla
                }
                else
                {
                    throw new Error("La variable " + Contenido + " no está definida", log, linea, columna);
                }
                match(Tipos.Identificador);
            }
            else if (Clasificacion == Tipos.Cadena)
            {
                resultado = Contenido.Trim('"');
                match(Tipos.Cadena);
            }
            if (Contenido == "+")
            {
                match("+");
                resultado += Concatenaciones();  // Acumula el siguiente fragmento de concatenación
            }
            return resultado;
        }
        //!SECTION
        //SECTION - Main
        //Main -> static void Main(string[] args) BloqueInstrucciones 
        private void Main()
        {
            match("static");
            match("void");
            match("Main");
            match("(");
            match("string");
            match("[");
            match("]");
            match("args");
            match(")");
            BloqueInstrucciones(true);
        }
        //!SECTION
        //SECTION - Expresion
        // Expresion -> Termino MasTermino
        private void Expresion()
        {
            Termino();
            MasTermino();
        }
        //!SECTION
        //SECTION - MasTermino
        //MasTermino -> (OperadorTermino Termino)?
        private void MasTermino()
        {
            if (Clasificacion == Tipos.OperadorTermino)
            {
                string operador = Contenido;
                match(Tipos.OperadorTermino);
                Termino();
                //Console.Write(operador + " ");
                float n1 = s.Pop();
                asm.WriteLine("\tPOP EAX");
                float n2 = s.Pop();
                asm.WriteLine("\tPOP EBX");
                float resultado = 0;
                switch (operador)
                {
                    case "+": resultado = n2 + n1; asm.WriteLine("\tADD EAX, EBX"); break;
                    case "-": resultado = n2 - n1; asm.WriteLine("\tSUB EAX, EBX"); break;
                }

                Variable.TipoDato tipoResultado = Variable.valorTipoDato(resultado, maximoTipo, huboCasteo);
                //Si uno de los valores es float, el resultado sera float
                if (maximoTipo == Variable.TipoDato.Float || tipoResultado == Variable.TipoDato.Float)
                {
                    tipoResultado = Variable.TipoDato.Float;
                }

                if (huboCasteo)
                {
                    maximoTipo = tipoCasteo;
                }
                else
                {
                    if (maximoTipo == Variable.TipoDato.Float || tipoResultado == Variable.TipoDato.Float)
                    {
                        maximoTipo = Variable.TipoDato.Float;
                    }
                    else if (maximoTipo < tipoResultado)
                    {
                        maximoTipo = tipoResultado;
                    }
                }

                //Hacemos el push al final ya con el resultado
                asm.WriteLine("\tPUSH EAX");
                s.Push(resultado);
            }
        }
        //!SECTION
        //SECTION - Termino
        //Termino -> Factor PorFactor
        private void Termino()
        {
            Factor();
            PorFactor();
        }
        //!SECTION
        //SECTION - PorFactor
        //PorFactor -> (OperadorFactor Factor)?
        private void PorFactor()
        {
            if (Clasificacion == Tipos.OperadorFactor)
            {
                string operador = Contenido;
                match(Tipos.OperadorFactor);
                Factor();
                //Console.Write(operador + " ");
                float n1 = s.Pop();
                asm.WriteLine("\tPOP EBX");
                float n2 = s.Pop();
                asm.WriteLine("\tPOP EAX");

                float resultado = 0;
                int flagAsm = 4;
                switch (operador)
                {
                    case "*": resultado = n2 * n1; asm.WriteLine("\tMUL EBX"); flagAsm = 0; break; //AX
                    case "/": resultado = n2 / n1; asm.WriteLine("\tXOR EDX, EDX"); asm.WriteLine("\tDIV EBX"); flagAsm = 1; break; //AL
                    case "%": resultado = n2 % n1; asm.WriteLine("\tXOR EDX, EDX"); asm.WriteLine("\tDIV EBX"); flagAsm = 2; break; //AH
                }
                Variable.TipoDato tipoResultado = Variable.valorTipoDato(resultado, maximoTipo, huboCasteo);
                if (maximoTipo == Variable.TipoDato.Float || tipoResultado == Variable.TipoDato.Float)
                {
                    tipoResultado = Variable.TipoDato.Float;
                }

                if (huboCasteo)
                {
                    maximoTipo = tipoCasteo;
                }
                else
                {
                    if (maximoTipo == Variable.TipoDato.Float || tipoResultado == Variable.TipoDato.Float)
                    {
                        maximoTipo = Variable.TipoDato.Float;
                    }
                    else if (maximoTipo < tipoResultado)
                    {
                        maximoTipo = tipoResultado;
                    }
                }

                switch (flagAsm)
                {
                    case 0: asm.WriteLine("\tPUSH EAX"); break;
                    case 1: asm.WriteLine("\tPUSH EAX"); break;
                    case 2: asm.WriteLine("\tPUSH EDX"); break;
                    case 4: break;
                }
                s.Push(resultado);
            }
        }
        //!SECTION

        //SECTION - FACTOR
        //Factor -> numero | identificador | (Expresion)
        private void Factor()
        {
            maximoTipo = Variable.TipoDato.Char;

            // Caso 1: Si es un número
            if (Clasificacion == Tipos.Numero)
            {
                Variable? v = l.Find(variable => variable.Nombre == Contenido);
                //Contenido lo pasamos a valor
                float valor = float.Parse(Contenido);
                Variable.TipoDato tipoValor = Variable.valorTipoDato(valor, maximoTipo, huboCasteo);
                // Verifica el tipo de dato máximo necesario para el número
                if (maximoTipo < tipoValor)
                {
                    maximoTipo = tipoValor;
                }

                // Agrega el número al stack
                asm.WriteLine("\tMOV EAX, " + Contenido);
                //NOTE - PUSH 6
                asm.WriteLine("\tPUSH EAX");
                s.Push(valor);
                match(Tipos.Numero);
            }
            // Caso 2: Si es un identificador (variable)
            else if (Clasificacion == Tipos.Identificador)
            {
                // Busca la variable en la lista de variables
                Variable? v = l.Find(variable => variable.Nombre == Contenido);
                if (v == null)
                {
                    throw new Error("Sintaxis: la variable " + Contenido + " no está definida", log, linea, columna);
                }

                // Actualiza el tipo máximo si es necesario
                if (maximoTipo < v.Tipo)
                {
                    maximoTipo = v.Tipo;
                }

                // Agrega el valor de la variable al stack
                s.Push(v.getValor());
                asm.WriteLine($"\tMOV EAX, [{Contenido}]");
                asm.WriteLine("\tPUSH EAX");
                match(Tipos.Identificador);
            }
            // Caso 3: Si es una expresión entre paréntesis
            else
            {
                match("(");
                //Casteo explicito
                huboCasteo = false;
                if (Clasificacion == Tipos.TipoDato)
                {
                    // Determina el tipo de casteo
                    switch (Contenido)
                    {
                        case "int": tipoCasteo = Variable.TipoDato.Int; break;
                        case "float": tipoCasteo = Variable.TipoDato.Float; break;
                        case "char": tipoCasteo = Variable.TipoDato.Char; break;
                    }

                    match(Tipos.TipoDato);
                    match(")");
                    match("(");
                    huboCasteo = true;
                }
                //!SECTION
                // Evalúa la expresión dentro de los paréntesis
                Expresion();
                if (huboCasteo)
                {
                    float valor = s.Pop();
                    asm.WriteLine("\tPOP");

                    switch (tipoCasteo)
                    {
                        case Variable.TipoDato.Int:
                            valor = valor % MathF.Pow(2, 16);
                            break;

                        case Variable.TipoDato.Char:
                            valor = valor % MathF.Pow(2, 8);
                            break;
                    }
                    //Obligamos el casteo
                    maximoTipo = tipoCasteo;
                    asm.WriteLine("\tPUSH");
                    s.Push(valor);
                }
                match(")");
            }
        }
        // Método auxiliar token
        private string[] CapturarIncrementoTokens()
        {
            List<string> tokensIncremento = new List<string>();

            while (Contenido != ")")
            {
                tokensIncremento.Add(Contenido);
                nextToken();
            }
            return tokensIncremento.ToArray();
        }

        // Método auxiliar
        private void ProcesarIncremento(bool execute, string[] tokensIncremento)
        {
            if (tokensIncremento.Length == 2)
            {
                string varName = tokensIncremento[0];
                string op = tokensIncremento[1];

                Variable? v = l.Find(variable => variable.Nombre == varName);
                if (v == null)
                {
                    throw new Error($"Sintaxis: La variable {varName} no está definida", log, linea, columna);
                }
                switch (op)
                {
                    case "++":
                        asm.WriteLine($"; Incremento variable: {v.Nombre} usando ADD");
                        asm.WriteLine($"\tmov eax, [{v.Nombre}]");
                        asm.WriteLine($"\tadd eax, 1");
                        asm.WriteLine($"\tmov [{v.Nombre}], eax");
                        float r = v.getValor() + 1;
                        v.setValor(r, linea, columna, log, Variable.valorTipoDato(r, maximoTipo));
                        break;
                    case "--":
                        asm.WriteLine($"; Decremento variable: {v.Nombre} usando SUB");
                        asm.WriteLine($"\tmov eax, [{v.Nombre}]");
                        asm.WriteLine($"\tsub eax, 1");
                        asm.WriteLine($"\tmov [{v.Nombre}], eax");
                        r = v.getValor() - 1;
                        v.setValor(r, linea, columna, log, Variable.valorTipoDato(r, maximoTipo));
                        break;
                    default:
                        throw new Error($"Sintaxis: Incremento no reconocido: {op}", log, linea, columna);
                }
            }
            else
            {
                string expr = string.Join(" ", tokensIncremento);
                Contenido = expr;
                Asignacion(execute);
            }
        }

        /*SNT = Producciones = Invocar el metodo
        ST  = Tokens (Contenido | Classification) = Invocar match    Variables -> tipo_dato Lista_identificadores; Variables?*/
    }
}

//SECTION - Cambios de esta version
/*
   
*/
//!SECTION