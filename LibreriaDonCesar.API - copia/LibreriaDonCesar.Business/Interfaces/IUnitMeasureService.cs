using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibreriaDonCesar.Business.DTOs;
using LibreriaDonCesar.core.Common;
using LibreriaDonCesar.Core.Entities;

namespace LibreriaDonCesar.Business.Interfaces
{
    public interface IUnitMeasureService
    {
        Task<ServiceResponse<IEnumerable<UnitMeasure>>> GetAllAsync();

        Task<ServiceResponse<UnitMeasure>> GetByIdAsync(int id);
        Task<ServiceResponse<UnitMeasure>> GetByNameAsync(string name);
        Task<ServiceResponse<UnitMeasure>> CreateAsync(CreateUnitMeasureDto newUnitMeasure);

        Task<ServiceResponse<UnitMeasure>> UpdateAsync(int id, UpdateUnitMeasureDto unitMeasure);
        Task<ServiceResponse<UnitMeasure>> SetStateAsync(int unitMeasureId, bool state);

    }
}
