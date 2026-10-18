namespace Interfaces;

using Personajes;

internal interface IPuedeRegenerarse {
    void Regenerarse();
}

internal interface IPuedeQuemar {
    void Quemar(List<Personaje> listaP);
}