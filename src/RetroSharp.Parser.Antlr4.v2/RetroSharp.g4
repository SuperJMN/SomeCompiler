grammar RetroSharp;

// We define the main rule that starts parsing the file
program: (variableDeclaration | externFunction | function | statement)* EOF;

// Variable declaration with optional inline initialization
variableDeclaration: type IDENTIFIER ('=' expression)? ';';

// Data types
// Extended to accept TinyCSharp-8bit core types and ptr<T> without changing semantics yet
// Existing code still treats type as text; we only broaden accepted syntax.
type: 'void' | 'int' | 'char' | 'string' | 'byte'
    | 'u8' | 'i8' | 'u16' | 'i16' | 'bool'
    | 'ptr' '<' type '>'
    ;

// Attributes (zero-cost hints; currently ignored by semantics)
attrs: ('[' IDENTIFIER ('(' arguments? ')')? ']')* ;

// Extern function declaration (prototype only)
externFunction: attrs? 'extern' type IDENTIFIER '(' parameters? ')' ';' ;

// Function definition (now supports optional attributes before the signature)
function: attrs? type IDENTIFIER '(' parameters? ')' block;

// Function parameters, separated by commas
parameters: parameter (',' parameter)*;
parameter: type IDENTIFIER;

// Code blocks, for functions, conditionals, and loops
block: '{' statement* '}';

// Statements that can appear within functions and blocks
statement: variableDeclaration
          | expression ';'
          | conditional
          | whileLoop
          | forLoop
          | block // Allows nested blocks
          | returnStatement
          ;

// Expressions
expression: assignment | conditionalOrExpression ;

assignment: lvalue '=' expression ;

conditionalOrExpression: conditionalOrExpression '||' conditionalAndExpression
                       | conditionalAndExpression ;

conditionalAndExpression: conditionalAndExpression '&&' equalityExpression
                        | equalityExpression ;

equalityExpression: equalityExpression ('==' | '!=') relationalExpression
                  | relationalExpression ;

relationalExpression: relationalExpression ('<' | '<=' | '>' | '>=') shiftExpression
                    | shiftExpression ;

// Shift operations (new level between relational and additive)
shiftExpression: shiftExpression ('<<' | '>>') addExpression
               | addExpression ;

addExpression: addExpression ('+' | '-') mulExpression
             | mulExpression ;

mulExpression: mulExpression ('*' | '/' | '%') unaryExpression
             | unaryExpression ;

unaryExpression: ('+' | '-' | '!' | '~') unaryExpression
               | primary ;

primary: '(' expression ')'
       | IDENTIFIER
       | functionCall
       | LITERAL ;

// LValue forms for assignments (parser-only for now)
lvalue: IDENTIFIER
      | '*' expression
      | IDENTIFIER '[' expression ']' ;

// Function call
functionCall: IDENTIFIER '(' arguments? ')';

// Arguments in function calls, separated by commas
arguments: expression (',' expression)*;

// Control structures
conditional: 'if' '(' expression ')' block ('else' block)?;
whileLoop: 'while' '(' expression ')' block;
forLoop: 'for' '(' (variableDeclaration | assignment)? ';' expression? ';' assignment? ')' block;

// Return statement
returnStatement: 'return' expression? ';';

// Tokens
IDENTIFIER: [a-zA-Z_][a-zA-Z_0-9]*;
LITERAL: LITERAL_INT | LITERAL_CHAR | LITERAL_STRING  | 'true' | 'false';
LITERAL_INT: [0-9]+;
LITERAL_CHAR: '\'' . '\'';
LITERAL_STRING: '"' ('\\"' | .)*? '"';

// Spaces and line breaks (ignored)
WS: [ \t\r\n]+ -> skip;
