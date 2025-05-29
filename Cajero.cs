using System;
using System.IO;
using System.Linq;

public struct CuentaBancaria
{
    public string numeroCuenta;
    public string nombreTitular;
    public string contrasena;
    public decimal saldo;
}

class Program
{
   	private const string ARCHIVO_CUENTAS = "C:\\Users\\USUARIO\\Downloads\\cuentas.txt";
    private const int MAX_INTENTOS = 3;

    static void Main(string[] args)
    {
        //Se cargan los registros del archivo en un array del tipo de la estructura
        CuentaBancaria[] cuentas = CargarDatos();

        Console.WriteLine("BIENVENIDO AL CAJERO AUTOMÁTICO");
        string numeroCuenta = "";
        int indiceCuenta = -1;
        
         bool cuentaValida = false;
        while (!cuentaValida)
        {
            Console.Write("Ingrese su número de cuenta (8 dígitos): ");
            string numeroCuentaRaw = Console.ReadLine();
            //QUE NO SEA NULLO
            if (numeroCuentaRaw != null)
			{
    			numeroCuenta = numeroCuentaRaw.Trim();
			}

            // Validar longitud
            if (numeroCuenta.Length != 8)
            {
                Console.WriteLine("Error: El número de cuenta debe tener exactamente 8 dígitos.");
                continue;
            }

            // Buscar la cuenta
            indiceCuenta = Array.FindIndex(cuentas, c => c.numeroCuenta == numeroCuenta);
            
            if (indiceCuenta == -1)
            {
                Console.WriteLine("¡Cuenta no encontrada! Por favor, intente de nuevo.");
            }
            else
            {
            	Console.Clear();
                cuentaValida = true;
            }
        }

        //Cargar los datos en la estructura en base al indice encontrado del numero de la cuenta
        CuentaBancaria cuentaActual = cuentas[indiceCuenta];

        // Autenticación con contraseña
        if (!validarUsuario(ref cuentaActual))
        {
            Console.WriteLine("Demasiados intentos fallidos. Saliendo del sistema...");
            return;
        }

        bool salir = false;
        //Mostrar menu hasta que la variable salir sea verdadera
        do
        {
            //Imprime el menu
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
                    if (montoDeposito > 0)
                    {
                        DepositarDinero(ref cuentaActual, montoDeposito);
                        cuentas[indiceCuenta] = cuentaActual;
                        GuardarDatos(cuentas);
                    }
                    else
                    {
                        Console.WriteLine("Monto inválido! Debe ser mayor a cero.");
                    }
                    break;

                case "3":
                    Console.Clear();
                    decimal montoRetiro = LeerMonto();
                    if (montoRetiro > 0)
                    {
                        // Note: Corrected variable name from 'cuenta.saldo' to 'cuentaActual.saldo' and 'monto' to 'montoRetiro'
                        if (cuentaActual.saldo >= montoRetiro) 
                        {
                            RetirarDinero(ref cuentaActual, montoRetiro);
                            cuentas[indiceCuenta] = cuentaActual;
                            GuardarDatos(cuentas);
                        }
                        else
                        {
                            Console.WriteLine("Saldo insuficiente!");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Monto inválido! Debe ser mayor a cero.");
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

        } while (!salir);
    }

    static bool validarUsuario(ref CuentaBancaria cuenta)
    {
        int intentos = 0;
        bool autenticado = false;

        while (intentos < MAX_INTENTOS && !autenticado)
        {
            Console.Write("Ingrese su contraseña: ");
            string contrasena = Console.ReadLine();

            if (contrasena == cuenta.contrasena)
            {
                autenticado = true;
                Console.Clear();
            }
            else
            {
                intentos++;
                Console.WriteLine(string.Format("Contraseña incorrecta. Intento {0} de {1}.", intentos, MAX_INTENTOS));
            }
        }

        return autenticado;
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
    	//Verifica que existe el archivo
        if (!File.Exists(ARCHIVO_CUENTAS)) return new CuentaBancaria[0];

        //Se cargan todas las lineas del archivo en una variable
        string[] lineas = File.ReadAllLines(ARCHIVO_CUENTAS);
        //array de tipo CuentaBancaria el cual tiene una longuitud en base a la cantidad de las lineas del archivo
        CuentaBancaria[] cuentas = new CuentaBancaria[lineas.Length];

        //por cada linea se separa los campos por medio del simbolo | 
        for (int i = 0; i < lineas.Length; i++)
        {
            string[] datos = lineas[i].Split('|');
            if (datos.Length >= 4)
            {
                cuentas[i].numeroCuenta = datos[0];
                cuentas[i].nombreTitular = datos[1];
                cuentas[i].saldo = decimal.Parse(datos[2]);
                cuentas[i].contrasena = datos[3];
            }
        }

        return cuentas;
    }

    static void GuardarDatos(CuentaBancaria[] cuentas)
    {
        string[] lineas = new string[cuentas.Length];
		//se actualiza el archivo en base a la informacion de las estructuras modificadas
        for (int i = 0; i < cuentas.Length; i++)
        {
            lineas[i] = string.Format("{0}|{1}|{2}|{3}", cuentas[i].numeroCuenta, cuentas[i].nombreTitular,
        	                          cuentas[i].saldo,cuentas[i].contrasena);
        }

        File.WriteAllLines(ARCHIVO_CUENTAS, lineas);
    }

    static void ConsultarSaldo(CuentaBancaria cuenta)
    {
    	//Se toma los datos de la estructura de tipo CuentaBancaria 
        Console.WriteLine(string.Format("\nTitular: {0}", cuenta.nombreTitular));
        Console.WriteLine(string.Format("Número de cuenta: {0}", cuenta.numeroCuenta));
        Console.WriteLine(string.Format("Saldo actual: {0:C}", cuenta.saldo));
    }

    static decimal LeerMonto()
    {
        Console.Write("\nIngrese el monto: ");
        decimal monto;
        decimal.TryParse(Console.ReadLine(), out monto);
        return monto;
    }

    //ref es una referencia a la variable que se manda como parametro fuera de la funcion, en este caso cualquier
    //cambio en este parametro afectara a la variable donde se mando a llamar la funcion
    static void DepositarDinero(ref CuentaBancaria cuenta, decimal monto)
    {
        cuenta.saldo += monto;
        
        Console.WriteLine(string.Format("\nDepósito exitoso de {0:C}", monto));
        Console.WriteLine(string.Format("Nuevo saldo: {0:C}", cuenta.saldo));
    }

    static void RetirarDinero(ref CuentaBancaria cuenta, decimal monto)
    {
        cuenta.saldo -= monto;
        Console.WriteLine(string.Format("\nRetiro exitoso de {0:C}", monto));
        Console.WriteLine(string.Format("Nuevo saldo: {0:C}", cuenta.saldo));
    }
}
