# Welcome!

This is "SomeCompiler", a little C-like compiler created for "The Joy of Learning®".

I'm making this in my free time to learn about compilers and some old-school topics. It's supposed to make me happier, but more often that not, it's making me get bald faster 🤣

## How does it work?

Currently, it's able to compile very simple programs into a sort of **intermediate language** (IL for short) that is nothing more than 3-Address Code. The benefit of this IL, is that it's so generic that it can virtually be translated to any platform.

Then, the IL is targeted to a specific platform with a **code generator**. It basically converts IL into the actual binary format (or aseembly code).

What can it do?

Right now, it can compile a few simple programs like this:
```
int main() 
{ 
    return 2 * 3 * 4; 
}
```

## Which platforms does it compile for?

It can compile for the Zilog Z80 processor only. I'm using it as a real platform to test the compiler 😊 But truth is that I almost know nothing about it apart from it being one of the most widely used processors of all time and part of systems like Nintendo Gameboy, Apple II, Commodor 64 and many others.
