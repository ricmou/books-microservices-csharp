using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using APIExemplar.Domain.Exemplars;
using APIExemplar.Domain.Shared;

namespace APIExemplar.Services
{
    public class ExemplarService : IExemplarService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IExemplarRepository _repo;

        public ExemplarService(IUnitOfWork unitOfWork, IExemplarRepository repo)
        {
            this._unitOfWork = unitOfWork;
            this._repo = repo;
        }

        public async Task<List<ExemplarDto>> GetAllAsync()
        {
            var list = await this._repo.GetAllAsync();

            List<ExemplarDto> listDto = list.ConvertAll<ExemplarDto>(exem => new ExemplarDto( exem.Id.AsString(), exem.Book.AsString(), exem.BookState.State, exem.SellerId.AsString(), exem.DateOfAcquisition.ToString()));

            return listDto;
        }

        public async Task<ExemplarDto> GetByIdAsync(ExemplarId id)
        {
            var exemplar = await this._repo.GetByIdAsync(id);
            
            if(exemplar == null)
                return null;

            return new ExemplarDto(exemplar.Id.AsString(), exemplar.Book.AsString(), exemplar.BookState.State,
                exemplar.SellerId.AsString(), exemplar.DateOfAcquisition.ToString());
        }

        public async Task<List<ExemplarDto>> GetByBookIdAsync(BookId id)
        {
            var list = await this._repo.GetExemplarsOfId(id);

            List<ExemplarDto> listDto = list.ConvertAll<ExemplarDto>(exem => new ExemplarDto( exem.Id.AsString(), exem.Book.AsString(), exem.BookState.State, exem.SellerId.AsString(), exem.DateOfAcquisition.ToString()));

            return listDto;
        }

        public async Task<List<ExemplarDto>> GetBySellerIdAsync(ClientId id)
        {
            var list = await this._repo.GetExemplarsOfClient(id);

            List<ExemplarDto> listDto = list.ConvertAll<ExemplarDto>(exem => new ExemplarDto( exem.Id.AsString(), exem.Book.AsString(), exem.BookState.State, exem.SellerId.AsString(), exem.DateOfAcquisition.ToString()));

            return listDto;
        }

        public async Task<ExemplarDto> AddAsync(CreatingExemplarDto dto)
        {
            //Create Value objects here
            var bookId = new BookId(dto.BookId);
            var exemplarState = new ExemplarState(dto.BookState);
            var clientId = new ClientId(dto.SellerId);
            
            var exemplar = new Exemplar(bookId, exemplarState, clientId, DateOnly.Parse(dto.DateOfAcquisition));

            await this._repo.AddAsync(exemplar);

            await this._unitOfWork.CommitAsync();

            return new ExemplarDto(exemplar.Id.AsString(), exemplar.Book.AsString(), exemplar.BookState.State,
                exemplar.SellerId.AsString(), exemplar.DateOfAcquisition.ToString());
        }

        public async Task<ExemplarDto> UpdateAsync(ExemplarDto dto)
        {
            var exemplar = await this._repo.GetByIdAsync(new ExemplarId(dto.ExemplarId)); 

            if (exemplar == null)
                return null;   

            // change all field
            exemplar.ChangeBook(new BookId(dto.BookId));
            exemplar.ChangeBookState(new ExemplarState(dto.BookState));
            exemplar.ChangeSellerId(new ClientId(dto.SellerId));
            exemplar.ChangeDateOfAcquisition(DateOnly.Parse(dto.DateOfAcquisition));

            await this._unitOfWork.CommitAsync();

            return new ExemplarDto(exemplar.Id.AsString(), exemplar.Book.AsString(), exemplar.BookState.State,
                exemplar.SellerId.AsString(), exemplar.DateOfAcquisition.ToString());
        }
        
        public async Task<ExemplarDto> DeleteAsync(ExemplarId id)
        {
            var exemplar = await this._repo.GetByIdAsync(id); 

            if (exemplar == null)
                return null;

            this._repo.Remove(exemplar);
            await this._unitOfWork.CommitAsync();

            return new ExemplarDto(exemplar.Id.AsString(), exemplar.Book.AsString(), exemplar.BookState.State,
                exemplar.SellerId.AsString(), exemplar.DateOfAcquisition.ToString());
        }
        
    }
}