namespace Personajes;

using Interfaces;

internal abstract class Personaje {
    public Personaje(string nombre) {
        Nombre = nombre;
    }

    public readonly string Nombre;

    private int _vida = 100;
    public int Vida {
        get => _vida;
        protected set => _vida = Math.Clamp(value, 0, 100);
    }

    public bool EstaVivo => _vida > 0;
    public bool MuerteAnunciada { get; set; } = false;

    public int Defensa { get; protected set; } = 100;

    public abstract int EnergiaBase { get; protected set; }
    public abstract int Energia { get; protected set; }
    public abstract int DanioBase { get; protected set; }
    public abstract int DanioAtaque { get; protected set; }
    public abstract string NombreHabilidadEspecial { get; protected set; }

    public int CalcularDanioRecibido(int ataque) {
        int danio = ataque - Defensa;
        return danio > 0 ? danio : 1;
    }

    // Ataque + contrataque.
    public void Atacar(Personaje objetivo) {
        int danioInfligido = objetivo.CalcularDanioRecibido(DanioAtaque);
        objetivo.RecibirDanio(DanioAtaque);
        Console.WriteLine($"{objetivo.Nombre} recibió {danioInfligido} de daño. Vida restante: {objetivo.Vida}.");

        int danioContra = CalcularDanioRecibido(objetivo.DanioAtaque);
        RecibirDanio(objetivo.DanioAtaque);
        Console.WriteLine($"El contraataque de {objetivo.Nombre} te hizo {danioContra} de daño. Vida restante: {Vida}.");
    }

    public abstract void HabilidadEspecial(Personaje objetivo);

    public void RecibirDanio(int danio) {
        Vida -= CalcularDanioRecibido(danio);
    }

    // Para la quemadura.
    public void RecibirDanioFijo(int danio) {
        Vida -= danio;
    }

    public void RestaurarEfectos() {
        Defensa = 100;
        DanioAtaque = DanioBase;
    }

    public void RestaurarValores() {
        Vida = 100;
        Energia = EnergiaBase;
        Defensa = 100;
        DanioAtaque = DanioBase;
        MuerteAnunciada = false;
    }
}

internal class Caballero : Personaje, IPuedeRegenerarse {
    public Caballero(string nombre) : base(nombre) { }

    public override int EnergiaBase { get; protected set; } = 100;
    public override int Energia { get; protected set; } = 100;
    public override int DanioBase { get; protected set; } = 115;
    public override int DanioAtaque { get; protected set; } = 115;
    public override string NombreHabilidadEspecial { get; protected set; } = "Escudo pesado";

    public override void HabilidadEspecial(Personaje objetivo) {
        if (Energia >= 20) {
            Energia -= 20;
            Defensa += 50 * Defensa / 100;
            Console.WriteLine("Habilidad activada.");
            Console.WriteLine("Tu defensa aumenta un 50% para este turno.");
        }
        else {
            Console.WriteLine("Energía insuficiente.");
        }

        Atacar(objetivo);
    }

    public void Regenerarse() {
        Vida += 10;
        Console.WriteLine($"{Nombre} se regeneró 10 de vida.");
    }
}

internal class Guerrero : Personaje, IPuedeQuemar {
    public Guerrero(string nombre) : base(nombre) { }

    public override int EnergiaBase { get; protected set; } = 80;
    public override int Energia { get; protected set; } = 80;
    public override int DanioBase { get; protected set; } = 125;
    public override int DanioAtaque { get; protected set; } = 125;
    public override string NombreHabilidadEspecial { get; protected set; } = "Furia berserker";

    public override void HabilidadEspecial(Personaje objetivo) {
        if (Energia >= 15) {
            Energia -= 15;
            int porcentaje = Vida >= 50 ? 25 : 50;
            DanioAtaque += porcentaje * DanioAtaque / 100;
            Console.WriteLine("Habilidad activada.");
            Console.WriteLine($"Tu ataque aumentó un {porcentaje}% para este turno.");
        }
        else {
            Console.WriteLine("Energía insuficiente.");
        }

        Atacar(objetivo);
    }

    public void Quemar(List<Personaje> listaP) {
        foreach (var p in listaP) {
            if (p != this && p.EstaVivo) {
                p.RecibirDanioFijo(5);
                Console.WriteLine($"{p.Nombre} recibió 5 de daño por quemadura. Vida restante: {p.Vida}.");
            }
        }
    }
}

internal class Mago : Personaje, IPuedeQuemar {
    public Mago(string nombre) : base(nombre) { }

    public override int EnergiaBase { get; protected set; } = 90;
    public override int Energia { get; protected set; } = 90;
    public override int DanioBase { get; protected set; } = 135;
    public override int DanioAtaque { get; protected set; } = 135;
    public override string NombreHabilidadEspecial { get; protected set; } = "Hechizo devastador";

    public override void HabilidadEspecial(Personaje objetivo) {
        if (Energia >= 40) {
            Energia -= 40;
            DanioAtaque += 50 * DanioAtaque / 100;
            Console.WriteLine("Habilidad activada.");
            Console.WriteLine("Tu ataque aumentó un 50% para este turno.");
        }
        else {
            Console.WriteLine("Energía insuficiente.");
        }

        Atacar(objetivo);
    }

    public void Quemar(List<Personaje> listaP) {
        foreach (var p in listaP) {
            if (p != this && p.EstaVivo) {
                p.RecibirDanioFijo(5);
                Console.WriteLine($"{p.Nombre} recibió 5 de daño por quemadura. Vida restante: {p.Vida}.");
            }
        }
    }
}

internal class Sacerdote : Personaje, IPuedeRegenerarse {
    public Sacerdote(string nombre) : base(nombre) { }

    public override int EnergiaBase { get; protected set; } = 120;
    public override int Energia { get; protected set; } = 120;
    public override int DanioBase { get; protected set; } = 105;
    public override int DanioAtaque { get; protected set; } = 105;
    public override string NombreHabilidadEspecial { get; protected set; } = "Curación divina";

    public override void HabilidadEspecial(Personaje objetivo) {
        if (Energia >= 25) {
            Energia -= 25;
            Vida += 30;
            Console.WriteLine("Habilidad activada.");
            Console.WriteLine("Te curaste 30 de vida con tu habilidad especial.");
        }
        else {
            Console.WriteLine("Energía insuficiente.");
        }

        Atacar(objetivo);
    }

    public void Regenerarse() {
        Vida += 10;
        Console.WriteLine($"{Nombre} se regeneró 10 de vida.");
    }
}

internal class Arquero : Personaje {
    public Arquero(string nombre) : base(nombre) { }

    public override int EnergiaBase { get; protected set; } = 70;
    public override int Energia { get; protected set; } = 70;
    public override int DanioBase { get; protected set; } = 120;
    public override int DanioAtaque { get; protected set; } = 120;
    public override string NombreHabilidadEspecial { get; protected set; } = "Tiro a distancia";

    public override void HabilidadEspecial(Personaje objetivo) {
        if (Energia >= 25) {
            Energia -= 25;
            Console.WriteLine("Habilidad activada.");

            int danioInfligido = objetivo.CalcularDanioRecibido(DanioAtaque);
            objetivo.RecibirDanio(DanioAtaque);
            Console.WriteLine($"{objetivo.Nombre} recibió {danioInfligido} de daño. Vida restante: {objetivo.Vida}.");
            Console.WriteLine($"Esquivaste el contraataque de {objetivo.Nombre}.");
        }
        else {
            Console.WriteLine("Energía insuficiente.");
            Atacar(objetivo);
        }
    }
}