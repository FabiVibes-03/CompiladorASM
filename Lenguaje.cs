/*
    //SECTION REQUERIMIENTOS:
    1) Declarar las variables en ensamblador con su tipo de dato
    2) En asignacion generar codigo ensamblador para ++ (inc) -- (dec)
    3) Para +=
    4) Generar codigo para console.Write y Line
    5) Console.Read y ReadLine
    6) Generar codigo ensamblador para if, else 
    7) Para el while
    8) P el for 
    9) Condicionar todos los set valor (if(execute))


    //TODO - PARAMETRIZAR
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
        Stack<float> s;
        List<Variable> l;
        Variable.TipoDato maximoTipo;
        bool huboCasteo = false;
        Variable.TipoDato tipoCasteo = Variable.TipoDato.Char;
        //!@params

        public Lenguaje() : base()
        {
            s = new Stack<float>();
            l = new List<Variable>();
            log.WriteLine("Constructor lenguaje");
            maximoTipo = Variable.TipoDato.Char;
        }
        public Lenguaje(string nombre) : base(nombre)
        {
            s = new Stack<float>();
            l = new List<Variable>();
            log.WriteLine("Constructor lenguaje");
            maximoTipo = Variable.TipoDato.Char;
        }


        private void displayStack()
        {
            Console.WriteLine("Contenido del stack: ");
            foreach (float elemento in s)
            {
                Console.WriteLine(elemento);
            }
        }

        private void displayLista()
        {
            asm.WriteLine("SECTION .DATA");
            log.WriteLine("Lista de variables: ");
            foreach (Variable elemento in l)
            {
                log.WriteLine($"{elemento.getNombre()} {elemento.GetTipoDato()} {elemento.getValor()}");
                asm.WriteLine($"{elemento.getNombre()} DW 0");
            }
        }

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
            displayLista();
        }
        //Librerias -> using ListaLibrerias; Librerias?

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

        private void Variables()
        {
            Variable.TipoDato t = Variable.TipoDato.Char;
            switch (Contenido)
            {
                case "int": t = Variable.TipoDato.Int; break;
                case "float": t = Variable.TipoDato.Float; break;
                    /* case "char": t = Variable.TipoDato.Char; break; */
            }
            match(Tipos.TipoDato);
            ListaIdentificadores(t);
            match(";");
            if (Clasificacion == Tipos.TipoDato)
            {
                Variables();
            }
        }
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
        //ListaIdentificadores -> identificador (= Expresion)? (,ListaIdentificadores)?
        private void ListaIdentificadores(Variable.TipoDato t)
        {
            if (l.Find(variable => variable.getNombre() == Contenido) != null)
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
                    //NOTE - Este pop se agregaba hasta el final del asm
                    //asm.WriteLine("    POP");
                    v.setValor(resultado, linea, columna, log, maximoTipo);
                }
            }
            if (Contenido == ",")
            {
                match(",");
                ListaIdentificadores(t);
            }
        }
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

        //Instruccion -> console | If | While | do | For | Variables | Asignación
        private void Instruccion(bool ejecuta)
        {
            switch (Contenido)
            {
                case "Console":
                    console(ejecuta);
                    break;
                case "if":
                    If(ejecuta);
                    break;
                case "while":
                    While(ejecuta);
                    break;
                case "do":
                    Do(ejecuta);
                    break;
                case "for":
                    For(ejecuta);
                    break;
                default:
                    if (Clasificacion == Tipos.TipoDato)
                    {
                        Variables();
                    }
                    else
                    {
                        Asignacion();
                        match(";");
                    }
                    break;
            }
        }
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
            Variable? v = l.Find(variable => variable.getNombre() == Contenido);
            if (v == null)
            {
                throw new Error($"Sintaxis: La variable {Contenido} no está definida", log, linea, columna);
            }
            //Console.Write(Contenido + " = ");
            match(Tipos.Identificador);
            switch (Contenido)
            {
                case "++":
                    match("++");
                    r = v.getValor() + 1;
                    v.setValor(r, linea, columna, log, maximoTipo);
                    break;
                case "--":
                    match("--");
                    r = v.getValor() - 1;
                    v.setValor(r, linea, columna, log, maximoTipo);
                    break;
                case "=":
                    match("=");
                    if (Contenido == "Console")
                    {
                        ListaIdentificadores(v.GetTipoDato()); // Ya se hace este procedimiento arriba así que simplemente obtenemos a través del método lo que necesitamos
                    }
                    else
                    {
                        asm.WriteLine($"; Asignacion de {v.getNombre()}");
                        Expresion();
                        r = s.Pop();
                        //NOTE - Checar
                        asm.WriteLine("    POP EAX");
                        asm.WriteLine($"    MOV DWORD[{v.getNombre()}], EAX");
                        maximoTipo = Variable.valorTipoDato(r, maximoTipo, huboCasteo);
                        v.setValor(r, linea, columna, log, maximoTipo);
                    }
                    break;
                case "+=":
                    match("+=");
                    Expresion();
                    r = v.getValor() + s.Pop();
                    asm.WriteLine("    POP 2");
                    maximoTipo = Variable.valorTipoDato(r, maximoTipo, huboCasteo);
                    v.setValor(r, linea, columna, log, maximoTipo);
                    break;
                case "-=":
                    match("-=");
                    Expresion();
                    r = v.getValor() - s.Pop();
                    asm.WriteLine("    POP 3");
                    maximoTipo = Variable.valorTipoDato(r, maximoTipo, huboCasteo);
                    v.setValor(r, linea, columna, log, maximoTipo);
                    break;
                case "*=":
                    match("*=");
                    Expresion();
                    r = v.getValor() * s.Pop();
                    asm.WriteLine("    POP 4");
                    maximoTipo = Variable.valorTipoDato(r, maximoTipo, huboCasteo);
                    v.setValor(r, linea, columna, log, maximoTipo);
                    break;
                case "/=":
                    match("/=");
                    Expresion();
                    r = v.getValor() / s.Pop();
                    asm.WriteLine("    POP 5");
                    maximoTipo = Variable.valorTipoDato(r, maximoTipo, huboCasteo);
                    v.setValor(r, linea, columna, log, maximoTipo);
                    break;
                case "%=":
                    match("%=");
                    Expresion();
                    r = v.getValor() % s.Pop();
                    asm.WriteLine("    POP 6");
                    maximoTipo = Variable.valorTipoDato(r, maximoTipo, huboCasteo);
                    v.setValor(r, linea, columna, log, maximoTipo);
                    break;
            }
            //displayStack();
        }

        //!SECTION

        /*If -> if (Condicion) bloqueInstrucciones | instruccion
        (else bloqueInstrucciones | instruccion)?*/
        private void If(bool ejecuta2)
        {
            match("if");
            match("(");
            bool ejecuta = Condicion() && ejecuta2;
            //Console.WriteLine(ejecuta);
            match(")");
            if (Contenido == "{")
            {
                BloqueInstrucciones(ejecuta);
            }
            else
            {
                Instruccion(ejecuta);
            }
            if (Contenido == "else")
            {
                match("else");
                bool ejecutarElse = !ejecuta; // Solo se ejecuta el else si el if no se ejecutó
                if (Contenido == "{")
                {
                    BloqueInstrucciones(ejecutarElse);
                }
                else
                {
                    Instruccion(ejecutarElse);
                }
            }
        }
        //Condicion -> Expresion operadorRelacional Expresion
        private bool Condicion()
        {
            maximoTipo = Variable.TipoDato.Char;

            Expresion();
            float valor1 = s.Pop();
            asm.WriteLine("    POP EBX");
            string operador = Contenido;
            match(Tipos.OperadorRelacional);

            maximoTipo = Variable.TipoDato.Char;

            Expresion();
            float valor2 = s.Pop();
            asm.WriteLine("    POP EAX");

            asm.WriteLine("    CMP EAX, EBX");
            switch (operador)
            {
                case ">": return valor1 > valor2;
                case ">=": return valor1 >= valor2;
                case "<": return valor1 < valor2;
                case "<=": return valor1 <= valor2;
                case "==": return valor1 == valor2;
                default: return valor1 != valor2;
            }
        }
        //While -> while(Condicion) bloqueInstrucciones | instruccion
        private void While()
        {
            match("while");
            match("(");
            Condicion();
            match(")");
            if (Contenido == "{")
            {
                BloqueInstrucciones(true);
            }
            else
            {
                Instruccion(true);
            }
        }
        /*Do -> do bloqueInstrucciones | intruccion 
        while(Condicion);*/
        private void Do()
        {
            match("do");
            if (Contenido == "{")
            {
                BloqueInstrucciones(true);
            }
            else
            {
                Instruccion(true);
            }
            match("while");
            match("(");
            Condicion();
            match(")");
            match(";");
        }
        /*For -> for(Asignacion; Condicion; Asignacion) 
        BloqueInstrucciones | Intruccion*/
        private void For()
        {
            match("for");
            match("(");
            Asignacion();
            match(";");
            Condicion();
            match(";");
            Asignacion();
            match(")");
            if (Contenido == "{")
            {
                BloqueInstrucciones(true);
            }
            else
            {
                Instruccion(true);
            }
        }
        //Console -> Console.(WriteLine|Write) (cadena? concatenaciones?);
        private void console(bool excecute)
        {
            bool console = false;
            bool isRead = false;
            string content = "";

            match("Console");
            match(".");

            switch (Contenido)
            {
                case "Write":
                    console = true;
                    match("Write");
                    break;
                case "Read":
                    isRead = true;
                    match("Read");
                    break;
                case "ReadLine":
                    isRead = true;
                    match("ReadLine");
                    break;
                default:
                    match("WriteLine");
                    break;
            }

            match("(");

            if (!isRead && Contenido != ")")
            {

                if (Clasificacion == Tipos.Cadena)
                {
                    if (excecute)
                    {
                        Console.Write(Contenido.ToString().Replace('"', ' '));
                    }
                    match(Tipos.Cadena);
                }
                else
                {
                    string nomV = Contenido;
                    match(Tipos.Identificador);
                    Variable v = l.Find(variable => variable.getNombre() == nomV);

                    if (v == null)
                    {
                        throw new Error("La variable no existe", log, linea, columna);
                    }
                    if (excecute)
                    {
                        //? Por alguna razón sigue imprimiendo en float REVISAR
                        Console.Write(((int)v.getValor()).ToString());
                    }
                    //match(v.getValor().ToString());
                }
            }

            if (Contenido == "+")
            {
                match("+");
                Concatenaciones();
            }

            match(")");
            match(";");

            string txt;
            int num;


            if (isRead)
            {
                switch (Contenido)
                {
                    case "ReadLine":
                        content = Console.ReadLine();
                        break;
                    case "Read":
                        content = Console.Read().ToString();
                        break;
                }

                if (!int.TryParse(content, out num))
                {
                    throw new Error("Error: La entrada no es un número válido.", log, linea, columna);
                }
            }



            if (!isRead && excecute)
            {
                switch (console)
                {
                    case true: Console.Write(content); break;
                    case false: Console.WriteLine(content); break;
                }
            }
        }
        // Concatenaciones -> Identificador|Cadena ( + concatenaciones )?
        private string Concatenaciones()
        {
            string resultado = "";
            if (Clasificacion == Tipos.Identificador)
            {
                Variable? v = l.Find(variable => variable.getNombre() == Contenido);
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
        // Expresion -> Termino MasTermino
        private void Expresion()
        {
            Termino();
            MasTermino();
        }
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
                asm.WriteLine("    POP EAX");
                float n2 = s.Pop();
                asm.WriteLine("    POP EBX");
                float resultado = 0;
                switch (operador)
                {
                    case "+": resultado = n2 + n1; asm.WriteLine("     ADD EBX, EAX"); break;
                    case "-": resultado = n2 - n1; asm.WriteLine("     SUB EBX, EAX"); break;
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

                asm.WriteLine("     PUSH EBX");
                s.Push(resultado);
            }
        }
        //Termino -> Factor PorFactor
        private void Termino()
        {
            Factor();
            PorFactor();
        }
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
                asm.WriteLine("    POP EBX");
                float n2 = s.Pop();
                asm.WriteLine("    POP EAX");

                float resultado = 0;
                int flagAsm = 4;
                switch (operador)
                {
                    case "*": resultado = n2 * n1; asm.WriteLine("     MUL EBX"); flagAsm = 0; break; //AX
                    case "/": resultado = n2 / n1; asm.WriteLine("    XOR EDX, EDX"); asm.WriteLine("     DIV EBX"); flagAsm = 1; break; //AL
                    case "%": resultado = n2 % n1; asm.WriteLine("    XOR EDX, EDX"); asm.WriteLine("     DIV EBX"); flagAsm = 2; break; //AH
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
                    case 0: asm.WriteLine("     PUSH EAX"); break;
                    case 1: asm.WriteLine("     PUSH EAX"); break;
                    case 2: asm.WriteLine("     PUSH EDX"); break;
                    case 4: break;
                }
                s.Push(resultado);
            }
        }

        //SECTION - FACTOR
        //Factor -> numero | identificador | (Expresion)
        private void Factor()
        {
            maximoTipo = Variable.TipoDato.Char;
            // Caso 1: Si es un número
            if (Clasificacion == Tipos.Numero)
            {
                //Contenido lo pasamos a valor
                float valor = float.Parse(Contenido);
                Variable.TipoDato tipoValor = Variable.valorTipoDato(valor, maximoTipo, huboCasteo);
                // Verifica el tipo de dato máximo necesario para el número
                if (maximoTipo < tipoValor)
                {
                    maximoTipo = tipoValor;
                }

                // Agrega el número al stack
                asm.WriteLine("     MOV EAX, " + Contenido);
                asm.WriteLine("     PUSH EAX");
                s.Push(valor);
                match(Tipos.Numero);
            }
            // Caso 2: Si es un identificador (variable)
            else if (Clasificacion == Tipos.Identificador)
            {
                // Busca la variable en la lista de variables
                Variable? v = l.Find(variable => variable.getNombre() == Contenido);
                if (v == null)
                {
                    throw new Error("Sintaxis: la variable " + Contenido + " no está definida", log, linea, columna);
                }

                // Actualiza el tipo máximo si es necesario
                if (maximoTipo < v.GetTipoDato())
                {
                    maximoTipo = v.GetTipoDato();
                }

                // Agrega el valor de la variable al stack
                s.Push(v.getValor());
                asm.WriteLine("     MOV EAX, " + Contenido);
                asm.WriteLine("     PUSH EAX");
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
                    asm.WriteLine("    POP");

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
                    asm.WriteLine("     PUSH");
                    s.Push(valor);
                }
                match(")");
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