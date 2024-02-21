﻿grammar SomeLanguage;

// We define the main rule that starts parsing the file
program: (variableDeclaration | function | statement)* EOF;

// Variable declaration with optional inline initialization
variableDeclaration: type IDENTIFIER ('=' expression)? ';';

// Data types
type: 'void' | 'int' | 'char' | 'string' | 'byte';

// Function definition
function: type IDENTIFIER '(' parameters? ')' block;

// Function parameters, separated by commas
parameters: parameter (',' parameter)*;
parameter: type IDENTIFIER;

// Code blocks, for functions, conditionals, and loops
block: '{' statement* '}';

// Statements that can appear within functions and blocks
statement: variableDeclaration
          | assignment
          | functionCall ';'
          | conditional
          | whileLoop
          | block // Allows nested blocks
          | returnStatement
          ;

// Assignment of values to variables
assignment: IDENTIFIER '=' expression ';';

// Expressions
expression: addExpression;

addExpression: addExpression ('+' | '-') mulExpression
             | mulExpression
             ;

mulExpression: mulExpression ('*' | '/') atom
             | atom
             ;

atom: '(' expression ')'
    | IDENTIFIER
    | functionCall
    | LITERAL
    ;

// Function call
functionCall: IDENTIFIER '(' arguments? ')';

// Arguments in function calls, separated by commas
arguments: expression (',' expression)*;

// Control structures
conditional: 'if' '(' expression ')' block ('else' block)?;
whileLoop: 'while' '(' expression ')' block;

// Return 
returnStatement: 'return' expression? ';';

// Tokens
IDENTIFIER: [a-zA-Z_][a-zA-Z_0-9]*;
LITERAL: LITERAL_INT | LITERAL_CHAR | LITERAL_STRING;
LITERAL_INT: [0-9]+;
LITERAL_CHAR: '\'' . '\'';
LITERAL_STRING: '"' ('\\"' | .)*? '"';

// Spaces and line breaks (ignored)
WS: [ \t\r\n]+ -> skip;
