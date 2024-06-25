using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.IServices
{
    public interface IEnkriptimi
    {
        string Encrypt(string plainText);
        string Decrypt(string plainText);
    }
}
