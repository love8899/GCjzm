using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Wfm.Core.Domain.Common;


namespace Wfm.Services.Common
{
    public partial interface IPositionService
    {

        void InsertPosition(Position position);

        void UpdatePosition(Position position);

        void DeletePosition(Position position);

        Position GetPositionById(int? id);

        Position GetPositionByGuid(Guid guid);

        int GetPositionIdByName(string name);

        IList<Position> GetAllPositions();

        IList<Position> GetAllPositionByCompanyId(int companyId);

        IList<Position> GetAllPositionByCompanyGuid(Guid? guid);

        void DeleteAllPositionsByCompanyGuid(Guid? guid);
    }
}
