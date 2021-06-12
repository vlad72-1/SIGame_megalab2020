using System;

namespace SIGameLibrary
{
    public class ObjectCreator
    {
        public static dynamic Create(string progID, params object[] parameters)  // Создать объект по ProgID
        {
            Type type = Type.GetTypeFromProgID(progID, true);  
            dynamic dispatcher = ((dynamic)Activator.CreateInstance(type)).Assign(parameters);  
            return dispatcher;  
        }
    }
}
