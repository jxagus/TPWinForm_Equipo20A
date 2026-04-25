using dominio;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace negocio
{
    public class ArticuloNegocio
    {
        public List<Articulo> listar()
        {
            List<Articulo> lista = new List<Articulo>();
            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.setearConsulta(
                    "SELECT A.Id, A.Codigo, A.Nombre, A.Descripcion, A.Precio, " +
                    "M.Descripcion AS DescripcionMarca, " +
                    "C.Descripcion AS DescripcionCategoria, " +
                    "M.Id AS IdMarca, C.Id AS IdCategoria " +
                    "FROM ARTICULOS A " +
                    "LEFT JOIN MARCAS M ON A.IdMarca = M.Id " +
                    "LEFT JOIN CATEGORIAS C ON A.IdCategoria = C.Id");

                datos.ejecutarLectura();

                while (datos.Lector.Read())
                {
                    Articulo aux = new Articulo();
                    aux.Id = (int)datos.Lector["Id"];
                    aux.Codigo = (string)datos.Lector["Codigo"];
                    aux.Nombre = (string)datos.Lector["Nombre"];
                    aux.Descripcion = (string)datos.Lector["Descripcion"];
                    aux.Precio = Convert.ToDecimal(datos.Lector["Precio"]);

                    aux.Marca = new Marca();
                    if (!(datos.Lector["IdMarca"] is DBNull))
                    {
                        aux.Marca.Id = (int)datos.Lector["IdMarca"];
                        aux.Marca.Descripcion = (string)datos.Lector["DescripcionMarca"];
                    }
                    else
                    {
                        aux.Marca.Descripcion = "Sin Marca";
                    }

                        aux.Categoria = new Categoria();
                    if (!(datos.Lector["IdCategoria"] is DBNull))
                    {
                        aux.Categoria.Id = (int)datos.Lector["IdCategoria"];
                        aux.Categoria.Descripcion = (string)datos.Lector["DescripcionCategoria"];
                    }
                    else
                    {
                        aux.Categoria.Descripcion = "Sin Categoría";
                    }

                    ImagenNegocio imgNegocio = new ImagenNegocio();
                    aux.Imagenes = imgNegocio.listar(aux.Id);

                    lista.Add(aux);
                }

                return lista;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                datos.cerrarConexion();
            }
        
        }

        public void agregar(Articulo nuevo)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta("insert into ARTICULOS (Codigo, Nombre, Descripcion, IdMarca, IdCategoria, Precio) " + "output inserted.id " + "values('" + nuevo.Codigo + "', '" + nuevo.Nombre + "', '" + nuevo.Descripcion + "', @idMarca, @idCategoria, @Precio)");
                datos.setearParametro("@idMarca", nuevo.Marca.Id);
                datos.setearParametro("@idCategoria", nuevo.Categoria.Id);
                datos.setearParametro("@Precio", nuevo.Precio);
                int idArticulo = datos.leerUltimoId();
                foreach (Imagen img in nuevo.Imagenes)
                {
                    datos.limpiarParametros();
                    datos.setearConsulta("insert into IMAGENES (IdArticulo, ImagenUrl) values (@idArt, @urlImagen)");
                    datos.setearParametro("@idArt", idArticulo);
                    datos.setearParametro("@urlImagen", img.UrlImagen);

                    datos.ejecutarAccion();
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        public void modificar(Articulo modificar)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta("update ARTICULOS set Codigo = @Codigo, Nombre = @Nombre, Descripcion = @Desc, IdMarca = @IdMarca, IdCategoria = @IdCat, Precio = @Precio where Id = @id");
                datos.setearParametro("@Codigo", modificar.Codigo);
                datos.setearParametro("@Nombre", modificar.Nombre);
                datos.setearParametro("@Desc", modificar.Descripcion);
                datos.setearParametro("@IdMarca", modificar.Marca.Id);
                datos.setearParametro("@IdCat", modificar.Categoria.Id);
                datos.setearParametro("@Precio", modificar.Precio);
                datos.setearParametro("@id", modificar.Id);
                datos.ejecutarAccion();

                datos.limpiarParametros();
                datos.setearConsulta("delete from IMAGENES where IdArticulo = @idArt");
                datos.setearParametro("@idArt", modificar.Id);
                datos.ejecutarAccion();

                foreach (Imagen img in modificar.Imagenes)
                {
                    datos.limpiarParametros();
                    datos.setearConsulta("insert into IMAGENES (IdArticulo, ImagenUrl) values (@idArt, @urlImagen)");
                    datos.setearParametro("@idArt", modificar.Id);
                    datos.setearParametro("@urlImagen", img.UrlImagen);

                    datos.ejecutarAccion();
                }


            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally { datos.cerrarConexion(); }
        }

        public List<Articulo> filtrar(string campo, string criterio, string filtro)
        {
            List<Articulo> listaArticulo = new List<Articulo>();
            AccesoDatos datos = new AccesoDatos();

            try
            {
                //se realizo un cambio en la consulta y en datos.lector
                string consulta = "SELECT A.Id, A.Codigo, A.Nombre, A.Descripcion, A.Precio, " +
                    "M.Descripcion AS DescripcionMarca, " +
                    "C.Descripcion AS DescripcionCategoria, " +
                    "M.Id AS IdMarca, C.Id AS IdCategoria " +
                    "FROM ARTICULOS A " +
                    "LEFT JOIN MARCAS M ON A.IdMarca = M.Id " +
                    "LEFT JOIN CATEGORIAS C ON A.IdCategoria = C.Id WHERE 1=1 "; //obligamos a que la consulta sea siempre valida

                switch (campo)
                {
                    case "Precio":
                        switch (criterio)
                        {
                            case "Mayor a":
                                consulta += "AND A.Precio > " + filtro;
                                break;
                            default:
                                consulta += "AND A.Precio < " + filtro;
                                break;

                        }
                        break;
                    case "Nombre":
                        switch (criterio)
                        {
                            case "Comienza con":
                                consulta += "AND Nombre like '" + filtro + "%' ";
                                break;
                            case "Termina con":
                                consulta += "AND Nombre like '%" + filtro + "' ";
                                break;
                            default:
                                consulta += "AND Nombre like '%" + filtro + "%' ";
                                break;
                        }
                        break;
                    case "Categoria":
                        if (filtro.ToLower() == "Sin categoría" || string.IsNullOrWhiteSpace(filtro))
                        {
                            consulta += " AND C.id  is null";
                        }
                        else
                        {
                            switch (criterio)
                            {
                                case "Comienza con":
                                    consulta += "AND C.Descripcion like '" + filtro + "%' ";
                                    break;
                                case "Termina con":
                                    consulta += "AND C.Descripcion like '%" + filtro + "' ";
                                    break;
                                default:
                                    consulta += "AND C.Descripcion like '%" + filtro + "%' ";
                                    break;   
                            }
                        }
                        break;
                    case "Marca":
                        if (filtro.ToLower() == "Sin categoría" || string.IsNullOrWhiteSpace(filtro))
                        {
                            consulta += " AND C.id  is null";
                        }
                        else
                        {
                            switch (criterio)
                            {
                                case "Comienza con":
                                    consulta += "AND M.Descripcion like '" + filtro + "%' ";
                                    break;
                                case "Termina con":
                                    consulta += "AND M.Descripcion like '%" + filtro + "' ";
                                    break;
                                default:
                                    consulta += "AND M.Descripcion like '%" + filtro + "%' ";
                                    break;
                            }
                        }
                        break;

                }
                datos.setearConsulta(consulta);
                datos.ejecutarLectura();
                while (datos.Lector.Read())
                {
                    Articulo aux = new Articulo();
                    aux.Id = (int)datos.Lector["Id"];
                    aux.Codigo = (string)datos.Lector["Codigo"];
                    aux.Nombre = (string)datos.Lector["Nombre"];
                    aux.Descripcion = (string)datos.Lector["Descripcion"];
                    aux.Precio = Convert.ToDecimal(datos.Lector["Precio"]);

                    aux.Marca = new Marca();
                    if (!(datos.Lector["IdMarca"] is DBNull))
                    {
                        aux.Marca.Id = (int)datos.Lector["IdMarca"];
                        aux.Marca.Descripcion = (string)datos.Lector["DescripcionMarca"];
                    }
                    else
                    {
                        aux.Marca.Descripcion = "Sin Marca";
                    }

                    aux.Categoria = new Categoria();
                    if (!(datos.Lector["IdCategoria"] is DBNull))
                    {
                        aux.Categoria.Id = (int)datos.Lector["IdCategoria"];
                        aux.Categoria.Descripcion = (string)datos.Lector["DescripcionCategoria"];
                    }
                    else
                    {
                        aux.Categoria.Descripcion = "Sin Categoría";
                    }

                    ImagenNegocio imgNegocio = new ImagenNegocio();
                    aux.Imagenes = imgNegocio.listar(aux.Id);

                    listaArticulo.Add(aux);
                }
                return listaArticulo;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        
        public void Eliminar (int id)
        {
            try
            {
                AccesoDatos datos = new AccesoDatos();
                datos.setearConsulta("DELETE FROM Articulos WHERE Id = @id");
                datos.setearParametro("@id", id);
                datos.ejecutarAccion();
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

    }
}
