using api;
using System.Text;
using UnityEngine;

#nullable enable
namespace api.Plugins
{
    class Logger: Client.IPlugin
    {
        StringBuilder _builder = new();
        public Packet? Process(Packet p)
        {
            _builder.Clear();
            _builder.Append("Packet recieved of type '");
            _builder.Append(p.Type);
            _builder.Append("'(");
            _builder.Append(p.Type.ToString());
            _builder.Append(") with content as follows:");
            _builder.Append(p.Content);

            Debug.Log(_builder.ToString());

            return p;
        }
    }
}
