using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Specifications;

namespace Talabat.Repository.Generic_Repository
{
    public static class SpecificationsEvaluator<TEntity> where TEntity : BaseEntity
    {
        public static IQueryable<TEntity> GetQuery(IQueryable<TEntity> inputQuery, ISpecifications<TEntity> spec)
        {
            var query = inputQuery;  // _dbContext.Products

            if (spec.Criteria is not null)
                query = query.Where(spec.Criteria);   // P => true && true

            // query = _dbContext.Products.Where(P => true && true)

            if (spec.OrderBy is not null)  
                query = query.OrderBy(spec.OrderBy);  // P => P.Name  

            // query = _dbContext.Products.Where(P => true && true).OrderBy(P => P.Name)


            else if (spec.OrderByDesc is not null)  
                query = query.OrderByDescending(spec.OrderByDesc);

            if (spec.IsPaginationEnabled)
                query = query.Skip(spec.Skip).Take(spec.Take);  

            // query = _dbContext.Products.Where(P => true && true).OrderBy(P => P.Name).Skip(5).Take(5);
            //  Includes
            // 1. P => P.Brand 
            // 2. P => P.Category


            query = spec.Includes.Aggregate(query,(current, includeExpression) => current.Include(includeExpression));

            // _dbContext.Products.Where(P => true && true).OrderBy(P => P.Name).Skip(5).Take(5).Include(P => P.Brand);
            // _dbContext.Products.Where(P => true && true).OrderBy(P => P.Name).Skip(5).Take(5).Include(P => P.Brand).Include(P => P.Category);

            return query;
        }
    }
}
