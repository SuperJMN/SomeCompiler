grammar SomeLanguage;

// Definimos la regla principal que comienza a parsear el archivo
programa: (declaracionVar | funcion | sentencia)* EOF;

// Declaración de variables
declaracionVar: tipo IDENTIFICADOR ';';

// Tipos de datos
tipo: 'void' | 'int' | 'char' | 'string' | 'byte';

// Definición de funciones
funcion: tipo IDENTIFICADOR '(' parametros? ')' bloque;

// Parámetros de función, separados por comas
parametros: parametro (',' parametro)*;
parametro: tipo IDENTIFICADOR;

// Bloques de código, para funciones, condicionales y bucles
bloque: '{' (declaracionVar | sentencia)* '}';

// Sentencias que pueden aparecer dentro de las funciones y bloques
sentencia: asignacion
          | llamadaFuncion ';'
          | condicional
          | bucleWhile
          ;

// Asignación de valores a variables
asignacion: IDENTIFICADOR '=' expresion ';';

// Expresiones
expresion: expresion ('+' | '-' | '*' | '/') expresion
         | '(' expresion ')'
         | IDENTIFICADOR
         | llamadaFuncion
         | LITERAL
         ;

// Llamada a funciones
llamadaFuncion: IDENTIFICADOR '(' argumentos? ')';

// Argumentos en llamadas a funciones, separados por comas
argumentos: expresion (',' expresion)*;

// Estructuras de control
condicional: 'if' '(' expresion ')' bloque ('else' bloque)?;
bucleWhile: 'while' '(' expresion ')' bloque;

// Tokens
IDENTIFICADOR: [a-zA-Z_][a-zA-Z_0-9]*;
LITERAL: LITERAL_INT | LITERAL_CHAR | LITERAL_STRING;
LITERAL_INT: [0-9]+;
LITERAL_CHAR: '\'' . '\'';
LITERAL_STRING: '"' ('\\"' | .)*? '"';

// Espacios y saltos de línea (ignorados)
WS: [ \t\r\n]+ -> skip;
