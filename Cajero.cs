using System;
using System.IO;
using System.Linq;

public struct CuentaBancaria
{
    public string numeroCuenta;
    public string nombreTitular;
    public decimal saldo;
}

class Program
{
    private const string ARCHIVO_CUENTAS = "cuentas.txt";

    static void Main(string[] args)
    {
        CuentaBancaria[] cuentas = CargarDatos();
        int indiceCuenta;
        CuentaBancaria cuentaActual;

        Console.WriteLine("BIENVENIDO AL CAJERO AUTOMÁTICO");
        while (true)
        {
            Console.Write("Ingrese su número de cuenta: ");
            string numeroCuenta = Console.ReadLine();
            indiceCuenta = Array.FindIndex(cuentas, c => c.numeroCuenta == numeroCuenta);

            if (indiceCuenta != -1)
            {
                cuentaActual = cuentas[indiceCuenta];
                break;
            }
            else
            {
                Console.Clear();
                Console.WriteLine("¡Cuenta no encontrada! Por favor, intente de nuevo.");
            }
        }
        bool salir = false;

        do
        {
            Console.Clear();
            string opcion = MostrarMenu();

            switch (opcion)
            {
                case "1":
                    Console.Clear();
                    ConsultarSaldo(cuentaActual);
                    break;

                case "2":
                    Console.Clear();
                    decimal montoDeposito = LeerMonto();
                    DepositarDinero(ref cuentaActual, montoDeposito);
                    cuentas[indiceCuenta] = cuentaActual;
                    GuardarDatos(cuentas);
                    break;

                case "3":
                    Console.Clear();
                    decimal montoRetiro = LeerMonto();
                    if (SaldoSuficiente(cuentaActual, montoRetiro))
                    {
                        RetirarDinero(ref cuentaActual, montoRetiro);
                        cuentas[indiceCuenta] = cuentaActual;
                        GuardarDatos(cuentas);
                    }
                    else
                    {
                        Console.WriteLine("Saldo insuficiente!");
                    }
                    break;

                case "4":
                    Console.Clear();
                    salir = true;
                    Console.WriteLine("Saliendo del sistema...");
                    break;

                default:
                    Console.WriteLine("Opción no válida!");
                    break;
            }
            if (!salir)
            {
                Console.WriteLine("\nPresione Enter para volver al menú...");
                Console.ReadLine();
            }

        } while (!salir);
    }

    static string MostrarMenu()
    {
        Console.WriteLine("\n--- MENÚ PRINCIPAL ---");
        Console.WriteLine("1. Consultar saldo");
        Console.WriteLine("2. Depositar dinero");
        Console.WriteLine("3. Retirar dinero");
        Console.WriteLine("4. Salir del sistema");
        Console.Write("Seleccione una opción: ");
        return Console.ReadLine();
    }

    static CuentaBancaria[] CargarDatos()
    {
        if (!File.Exists(ARCHIVO_CUENTAS)) return new CuentaBancaria[0];

        string[] lineas = File.ReadAllLines(ARCHIVO_CUENTAS);
        CuentaBancaria[] cuentas = new CuentaBancaria[lineas.Length];

        for (int i = 0; i < lineas.Length; i++)
        {
            string[] datos = lineas[i].Split('|');
            cuentas[i].numeroCuenta = datos[0];
            cuentas[i].nombreTitular = datos[1];
            cuentas[i].saldo = decimal.Parse(datos[2]);
        }

        return cuentas;
    }

    static void GuardarDatos(CuentaBancaria[] cuentas)
    {
        string[] lineas = new string[cuentas.Length];

        for (int i = 0; i < cuentas.Length; i++)
        {
            lineas[i] = $"{cuentas[i].numeroCuenta}|{cuentas[i].nombreTitular}|{cuentas[i].saldo}";
        }

        File.WriteAllLines(ARCHIVO_CUENTAS, lineas);
    }

    static void ConsultarSaldo(CuentaBancaria cuenta)
    {
        Console.WriteLine($"\nTitular: {cuenta.nombreTitular}");
        Console.WriteLine($"Número de cuenta: {cuenta.numeroCuenta}");
        Console.WriteLine($"Saldo actual: {cuenta.saldo:C}"); 
    }

    static decimal LeerMonto()
    {
        decimal monto;
        do
        {
            Console.Write("\nIngrese el monto: ");
            if (!decimal.TryParse(Console.ReadLine(), out monto) || monto <= 0)
            {
                Console.WriteLine("Error: Debe ingresar un monto numérico y positivo.");
                monto = 0; 
            }
        } while (monto <= 0);

        return monto;
    }


    static bool SaldoSuficiente(CuentaBancaria cuenta, decimal monto)
    {
        return cuenta.saldo >= monto;
    }

    static void DepositarDinero(ref CuentaBancaria cuenta, decimal monto)
    {
        cuenta.saldo += monto;
        Console.WriteLine($"\nDepósito exitoso de {monto:C}");
        Console.WriteLine($"Nuevo saldo: {cuenta.saldo:C}");
    }

    static void RetirarDinero(ref CuentaBancaria cuenta, decimal monto)
    {
        cuenta.saldo -= monto;
        Console.WriteLine($"\nRetiro exitoso de {monto:C}");
        Console.WriteLine($"Nuevo saldo: {cuenta.saldo:C}");
    }
}
