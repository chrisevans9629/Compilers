.class public ILTestProject.Program
{
.method public static int32 Square(int32 number) cil managed
{
.maxstack 2
ldarg.0
dup
mul
ret
}
    
.method public static int32 Factorial(int32 x) cil managed
{
.maxstack 2
ldarg.0
beq check
//Factorial(x-1) + x;

ret
check:
ldc.i4.1
ret

}
}
