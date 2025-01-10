using AutoMapper;
using Jogging.Domain.Exceptions;
using Jogging.Domain.Interfaces.RepositoryInterfaces;
using Jogging.Domain.Models;
using Jogging.Infrastructure2.Data;
using Jogging.Infrastructure2.Models;
using Microsoft.EntityFrameworkCore;

namespace Jogging.Infrastructure2.Repositories.MySqlRepositories {

    public class AddressRepository : IGenericRepo<AddressDom> {
        private readonly JoggingCcContext _context;
        private readonly IMapper _mapper;

        public AddressRepository(JoggingCcContext context, IMapper mapper) {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<AddressDom>> GetAllAsync() {
            var addresses = await _context.Addresses
                .ToListAsync();

            if (addresses.Count == 0) {
                throw new AddressException("No addresses found");
            }

            return _mapper.Map<List<AddressDom>>(addresses);
        }

        public async Task<AddressDom> GetByIdAsync(int id) {
            var address = await _context.Addresses
                .FirstOrDefaultAsync(a => a.Id == id);

            return address == null
                ? throw new AddressException("Address not found")
                : _mapper.Map<AddressDom>(address);
        }

        public async Task<AddressDom> AddAsync(AddressDom addressDom) {
            var address = _mapper.Map<AddressEF>(addressDom);

            _context.Addresses.Add(address);
            await _context.SaveChangesAsync();

            return _mapper.Map<AddressDom>(address);
        }

        public async Task<AddressDom> UpsertAsync(int? addressId, AddressDom updatedAddressDom) {
            AddressEF? currentAddress;

            if (addressId.HasValue) {
                currentAddress = await _context.Addresses
                    .FirstOrDefaultAsync(a => a.Id == addressId.Value);
            } else {
                currentAddress = await FindAddressByValues(updatedAddressDom);
            }

            if (currentAddress == null) {
                return await AddAsync(updatedAddressDom);
            }

            var mappedAddress = _mapper.Map<AddressEF>(updatedAddressDom);

            // Only update if there are changes
            if (!AreAddressesEqual(currentAddress, mappedAddress)) {
                currentAddress.Street = mappedAddress.Street;
                currentAddress.HouseNumber = mappedAddress.HouseNumber;
                currentAddress.City = mappedAddress.City;
                currentAddress.ZipCode = mappedAddress.ZipCode;

                await _context.SaveChangesAsync();
            }

            return _mapper.Map<AddressDom>(currentAddress);
        }

        public async Task<AddressDom> UpdateAsync(int addressId, AddressDom updatedAddressDom) {
            var currentAddress = await _context.Addresses
                .FirstOrDefaultAsync(a => a.Id == addressId);

            if (currentAddress == null) {
                throw new AddressException("Address not found");
            }

            var mappedAddress = _mapper.Map<AddressEF>(updatedAddressDom);

            currentAddress.Street = mappedAddress.Street;
            currentAddress.HouseNumber = mappedAddress.HouseNumber;
            currentAddress.City = mappedAddress.City;
            currentAddress.ZipCode = mappedAddress.ZipCode;

            await _context.SaveChangesAsync();

            return _mapper.Map<AddressDom>(currentAddress);
        }

        public async Task DeleteAsync(int addressId) {
            var address = await _context.Addresses
                .FirstOrDefaultAsync(a => a.Id == addressId);

            if (address == null) {
                throw new AddressException("Address not found");
            }

            _context.Addresses.Remove(address);
            await _context.SaveChangesAsync();
        }

        private async Task<AddressEF?> FindAddressByValues(AddressDom addressToFind) {
            return await _context.Addresses
                .FirstOrDefaultAsync(a =>
                    a.Street == addressToFind.Street &&
                    a.HouseNumber == addressToFind.HouseNumber &&
                    a.ZipCode == addressToFind.ZipCode &&
                    a.City == addressToFind.City);
        }

        private bool AreAddressesEqual(AddressEF current, AddressEF updated) {
            return current.Street == updated.Street &&
                   current.HouseNumber == updated.HouseNumber &&
                   current.City == updated.City &&
                   current.ZipCode == updated.ZipCode;
        }
    }
}