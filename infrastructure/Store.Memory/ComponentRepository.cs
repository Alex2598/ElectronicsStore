//using System.Collections.Generic;
//using System.Linq;

//namespace Store.Memory
//{
//    public class ComponentRepository : IComponentRepository
//    {
//        //private readonly Component[] components = new[]
//        //{
//        //    new Component(1, "10001", "0805", "Resistor",
//        //             "Simple Resistor",
//        //             7.19m),

//        //    new Component(2, "10002", "0805", "Capacitor",
//        //             "Simple Capacitor",
//        //             12.45m),

//        //    new Component(3, "10003", "0603, 0805", "Inductor",
//        //             "Simple Inductor ",
//        //             14.98m),
//        //};

//        public Component[] GetAllByIds(IEnumerable<int> componentIds)
//        {
//            var foundComponents = from component in components
//                             join componentId in componentIds on component.Id equals componentId
//                             select component;

//            return foundComponents.ToArray();
//        }

//        public Component[] GetAllByUid(string uid)
//        {
//            return components.Where(component => component.UId == uid)
//                        .ToArray();
//        }

//        public Component[] GetAllByPackageOrNameOfComponent(string query)
//        {
//            return components.Where(component => component.Package.Contains(query)
//                                    || component.NameOfComponent.Contains(query))
//                        .ToArray();
//        }

//        public Component GetById(int id)
//        {
//            return components.Single(component => component.Id == id);
//        }
//    }
//}
