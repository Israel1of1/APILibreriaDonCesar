using LibreriaDonCesar.core.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibreriaDonCesar.Business.Interfaces
{
    public interface IColorService
    {
        Task<ServiceResponse<IEnumerable<Color>>> GetAllAsync();
        Task<ServiceResponse<Color>> GetByIdAsync(int id);
        Task<ServiceResponse<Color>> GetByNameAsync(string name);
        Task<ServiceResponse<Color>> CreateAsync(CreateColorDto newColor);
        Task<ServiceResponse<Color>> UpdateAsync(int id, UpdateColorDto color);
    }
}
