using AuthServer.Core.Repositories;
using AuthServer.Core.Services;
using AuthServer.Core.UnitOfWork;
using AuthServer.Service.Mapper;
using AuthServer.SharedLibrary.Dtos;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection.Metadata.Ecma335;

namespace AuthServer.Service.Services
{
    public class GenericServices<T, TDto> : IGenericServices<T, TDto> where T : class where TDto : class
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly IGenericRepository<T> _genericRepository;

        public GenericServices(IUnitOfWork unitOfWork, IGenericRepository<T> genericRepository)
        {
            _unitOfWork = unitOfWork;
            _genericRepository = genericRepository;
        }

        public async Task<Response<TDto>> AddAsync(TDto entity)
        {
            var newEntity = ObjectMapper.Mapper.Map<T>(entity);
            await _genericRepository.AddAsync(newEntity);

            await _unitOfWork.CommitAsync();

            var newDto = ObjectMapper.Mapper.Map<TDto>(newEntity);

            return Response<TDto>.Success(newDto, 200);

        }

        public async Task<Response<NoDataDto>> Remove(int id)
        {
            var isExistEntity = await _genericRepository.GetByIdAsync(id);

            if (isExistEntity == null)
            {
                return Response<NoDataDto>.Fail("Id not found", 404, true);
            }

            _genericRepository.Remove(isExistEntity);
            await _unitOfWork.CommitAsync();
            // 204 durum kodu => No Content => Response Body'sinde hiç bir data olmayacaktır
            return Response<NoDataDto>.Success(204);
        }

        public async Task<Response<IQueryable<TDto>>> GetAllAsync()
        {
            var entity = ObjectMapper.Mapper.Map<List<TDto>>(await _genericRepository.GetAllAsync());

            return Response<IQueryable<TDto>>.Success(entity.AsQueryable(), 200);
        }

        public async Task<Response<TDto>> GetByIdAsync(int id)
        {
            var entity = await _genericRepository.GetByIdAsync(id);

            if (entity == null)
            {
                return Response<TDto>.Fail("Id not Found", 404, true);
            }

            return Response<TDto>.Success(ObjectMapper.Mapper.Map<TDto>(entity), 200);
        }

        public async Task<Response<NoDataDto>> Update(TDto entity,int id)
        {
            var isExistEntity = await _genericRepository.GetByIdAsync(id);

            if (isExistEntity == null) 
            {
                return Response<NoDataDto>.Fail("Id not Found", 404, true);
            }

            var updateEntity = ObjectMapper.Mapper.Map<T>(entity);

            _genericRepository.Update(updateEntity);
            await _unitOfWork.CommitAsync();
            // 204 durum kodu => No Content => Response Body'sinde hiç bir data olmayacaktır
            return Response<NoDataDto>.Success(204);
        }

        public async Task<Response<IQueryable<TDto>>> Where(Expression<Func<T, bool>> predicate)
        {
            //// where(x=>x.id>5) 
            var list = _genericRepository.Where(predicate);

            return Response<IQueryable<TDto>>.Success(ObjectMapper.Mapper.Map<IQueryable<TDto>>(await list.ToListAsync()), 200);
        }
    }
}
