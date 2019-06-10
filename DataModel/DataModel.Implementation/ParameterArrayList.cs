using System;
using System.Collections.Generic;

namespace DistributedExperimentation.DataModel.Implementation 
{
    public class ParameterArrayList : IParameterList
    {
        private String id;
        private String name;
        private String description;
        List<IParameter> parameters;

        private ParameterArrayList(String id, String name, String description, 
                                   List<IParameter> parameters) 
        {
            bool isOK = ((parameters != null) && (isValidId(id)) && 
                        (name != null) && (description != null));
            if (isOK) {
                this.id = id;
                this.name = name;
                this.description = description;
                this.parameters = parameters;
            } else {
                throw new ArgumentException("Arguments 'id', 'name', 'description', " +
                                            " and 'parameters' must be not null.\n" +
                                            "Argument 'id' must have a length between " +
                                            "1 and 60 characters and must consist " +
                                            "the characters 0-9a-zA-Z only.\n" +                                            
                                            "Argument 'parameters' must be a " +
                                            "valid list of IParameters.");
            }
        }

        public static ParameterArrayList create(String id, String name, String description, 
                                                List<IParameter> parameters) 
        {
            return new ParameterArrayList(id, name, description, parameters);
        }

        public static ParameterArrayList create(String id, String name, String description) 
        {
            return new ParameterArrayList(id, name, description, new List<IParameter>());
        }

        public void add(IParameter parameter)
        {
            if (parameter != null) {
                this.parameters.Add(parameter);
            } else {
                throw new ArgumentException("Argument 'parameter' is null.\n" +
                                            "Must be a valid Parameter.");                
            }
        }

        public void clear()
        {
            this.parameters.Clear();
        }

        public bool contains(IParameter parameter)
        {
            return this.parameters.Contains(parameter);
        }

        public uint count()
        {
            return (uint)this.parameters.Count;
        }

        public IParameter first()
        {
            return this.parameters[0];
        }

        public IParameter get(uint index)
        {
            return this.parameters[(int)index];
        }

        public uint indexOf(IParameter parameter)
        {
            if (parameter != null) {
                return (uint)this.parameters.IndexOf(parameter);
            } else {
                throw new ArgumentException("Argument 'parameter' is null.\n" +
                                            "Must be a valid Parameter.");                
            }
        }

        public void insert(uint index, IParameter parameter)
        {
            if (parameter != null) {
                this.parameters.Insert((int)index, parameter);
            } else {
                throw new ArgumentException("Argument 'parameter' is null.\n" +
                                            "Must be a valid Parameter.");                
            }
        }

        public bool isEmpty()
        {
            bool isEmpty = true;
            if (this.parameters.Count > 0) {
                isEmpty = false;
            }
            return isEmpty;
        }

        public bool isPrimitive()
        {
            return false;
        }

        public IParameter last()
        {
            return this.parameters[(int)(count()-1)];
        }

        public void remove(IParameter parameter)
        {
            if (parameter != null) {
                this.parameters.Remove(parameter);
            } else {
                throw new ArgumentException("Argument 'parameter' is null.\n" +
                                            "Must be a valid Parameter.");                
            }
        }

        public void removeAt(uint index)
        {
            this.parameters.RemoveAt((int)index);
        }
        
        public IParameter this[int index]
        {
            get {
                return this.parameters[index];
            }
            set {
                this.parameters[index] = value;
            }
        }

        public List<IParameter> toList() {
            return this.parameters;
        }

        public IParameter[] toArray() {
            return this.parameters.ToArray();
        }

        public string getId()
        {
            return this.id;
        }

        public string getName()
        {
            return this.name;
        }

        public string getDescription()
        {
            return this.description;
        }

        public string getValueTypeName()
        {
            return "parametercollection";
        }

        private bool isValidId(String id) 
        {
            return ((id != null) && 
                    (id.Length > 0) && (id.Length < 61) &&
                    (System.Text.RegularExpressions.Regex.IsMatch(id, "^([A-Za-z0-9])+$")));
        }        
    }
}