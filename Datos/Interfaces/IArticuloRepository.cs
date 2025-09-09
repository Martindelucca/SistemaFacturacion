using Practica01.Dominio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practica01.Datos.Interfaces
{
    public interface IArticuloRepository
    {
        List<Articulo> GetAll();
        Articulo GetById(int id);
        bool Save(Articulo articulo);
        bool Delete(int id);
    }
}
