using System.Reflection;
using System.Collections;
using LangMate.Extensions.Tools.Annotations;
using LangMate.Extensions.Extensions.String;
using LangMate.Extensions.Tools.DotEnvExceptions;

namespace LangMate.Extensions.Tools
{
    public static class DotEnv
    {
        public static void Load(string filePath = ".env")
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException(filePath);

            IDictionary<string, string> variable = DotEnvFile.LoadFile(filePath, true);
            DotEnvFile.InjectIntoEnvironment(EnvironmentVariableTarget.Process, variable);
        }

        public static T Load<T>() where T : new()
        {

            PropertyInfo[] propertyInfos;
            propertyInfos = typeof(T).GetProperties();

            T container = Activator.CreateInstance<T>();

            IDictionary variables = Environment.GetEnvironmentVariables();

            foreach (PropertyInfo property in propertyInfos)
            {
                EnvName envVarName = (EnvName)Attribute.GetCustomAttribute(property, typeof(EnvName));
                string key;
                if (envVarName?.Name != null)
                    key = envVarName.Name;
                else
                    key = property.Name.AddSpaceToPascalCase().ToConstantCase();

                if (variables.Contains(key))
                {
                    Type type = property.PropertyType;
                    //property.SetValue(container, (property.PropertyType) variables[key]);
                    property.SetValue(container, Convert.ChangeType(variables[key], property.PropertyType));
                }
                else
                    throw new EnvironmentVariableIsNotSetException(key);
            }

            return container;
        }

        public static string GetEnvName(this Type value, string propertyName)
        {
            return ((EnvName)Attribute.GetCustomAttribute(
                value.GetProperties().First(x => x.Name == propertyName), typeof(EnvName)
                )
            ).Name;
        }
    }
}
