using Personajes;
using Interfaces;

Random rnd = new();

bool salir = false;
List<Personaje> listaPersonajes = [];
bool revancha = false;

while (!salir) {
    // Creación de personajes:

    while (true) {
        if (revancha) break;
        Console.Write("Ingrese el nombre del personaje: ");
        string nombreCrea = Console.ReadLine() ?? "";

        Console.Write("""
            Escoja el rol del personaje:
            [1] Caballero.
            [2] Guerrero.
            [3] Mago.
            [4] Sacerdote.
            [5] Arquero.
            >>> 
            """);

        switch (ValidarOpcion(1, 5)) {
            case 1: listaPersonajes.Add(new Caballero(nombreCrea)); break;
            case 2: listaPersonajes.Add(new Guerrero(nombreCrea)); break;
            case 3: listaPersonajes.Add(new Mago(nombreCrea)); break;
            case 4: listaPersonajes.Add(new Sacerdote(nombreCrea)); break;
            case 5: listaPersonajes.Add(new Arquero(nombreCrea)); break;
        }

        Console.Write("¿Desea crear otro personaje? (Sí = 1, No = 0): ");

        if (ValidarOpcion(0, 1) == 0) {
            if (listaPersonajes.Count > 1)
                break;

            Console.WriteLine("Necesita al menos dos jugadores para empezar la partida.");
            continue;
        }
    }

    // Partida:
    while (true) {

        // Turnos:
        foreach (var p in listaPersonajes) {
            if (!p.EstaVivo) continue;

            Console.WriteLine($"""
                ------------------------------------------------------------------
                Nombre: {p.Nombre}.
                Vida: {p.Vida}.
                Energía: {p.Energia}.
                Daño: {p.DanioAtaque}.
                Habilidad especial: {p.NombreHabilidadEspecial}.
                ------------------------------------------------------------------
                """);

            Console.Write("¿Desea usar la habilidad especial? (Sí = 1, No = 0): ");

            // Elige el objetivo entre los vivos (excluyéndose):
            List<Personaje> enemigosVivos = [];
            foreach (var x in listaPersonajes) {
                if (x != p && x.EstaVivo)
                    enemigosVivos.Add(x);
            }

            if (enemigosVivos.Count == 0) break;

            Personaje objetivo = enemigosVivos[rnd.Next(0, enemigosVivos.Count)];

            if (ValidarOpcion(0, 1) == 0) p.Atacar(objetivo);
            else p.HabilidadEspecial(objetivo);

            AnunciarMuertes(listaPersonajes);

            Console.WriteLine("Presione una tecla para continuar.");
            Console.ReadKey();
            Console.WriteLine();
        }

        foreach (var restaurado in listaPersonajes)
            restaurado.RestaurarEfectos();

        Console.WriteLine("------------------------------------------------------------------");
        Console.WriteLine("Fase de cierre de ronda...");
        Console.WriteLine();


        // Efectos de final de ronda:
        foreach (var inter in listaPersonajes) {
            if (inter is IPuedeRegenerarse regenerable && inter.EstaVivo)
                regenerable.Regenerarse();

            if (inter is IPuedeQuemar quema && inter.EstaVivo)
                quema.Quemar(listaPersonajes);
        }

        AnunciarMuertes(listaPersonajes);

        // Verifica si terminó la partida:
        List<Personaje> jugadoresVivos = [];
        foreach (var p in listaPersonajes) {
            if (p.EstaVivo) jugadoresVivos.Add(p);
        }

        if (jugadoresVivos.Count <= 1) {
            if (jugadoresVivos.Count == 1)
                Console.WriteLine($"¡{jugadoresVivos[0].Nombre} se ha consagrado ganador/a de esta partida!");
            else
                Console.WriteLine("No quedan sobrevivientes. La partida terminó en empate.");
            break;
        }

        Console.WriteLine("Inicio de una nueva ronda.");
        Console.WriteLine("------------------------------------------------------------------");
    }

    Console.Write("""
        ------------------------------------------------------------------
        ¿Desea jugar otra partida?
        [1] Revancha (se mantienen los personajes).
        [2] Partida nueva.
        [3] Salir.
        >>> 
        """);

    switch (ValidarOpcion(1, 3)) {
        case 1:
            revancha = true;
            foreach (var perReset in listaPersonajes)
                perReset.RestaurarValores();
            break;
        case 2:
            listaPersonajes.Clear();
            revancha = false;
            break;
        case 3:
            salir = true;
            break;
    }
}


void AnunciarMuertes(List<Personaje> lista) {
    foreach (var p in lista) {
        if (!p.EstaVivo && !p.MuerteAnunciada) {
            Console.WriteLine($"{p.Nombre} ha muerto y está fuera de combate.");
            p.MuerteAnunciada = true;
        }
    }
}

int ValidarOpcion(int limMin, int limMax) {
    bool elecValida = int.TryParse(Console.ReadLine(), out int eleccion) && (eleccion >= limMin && eleccion <= limMax);
    while (!elecValida) {
        Console.Write("Opción inválida. Intente de nuevo: ");
        elecValida = int.TryParse(Console.ReadLine(), out eleccion) && (eleccion >= limMin && eleccion <= limMax);
    }
    return eleccion;
}