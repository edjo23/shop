using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Shop.Contracts.Services;

namespace Card.Service.Business
{
    public class CardWriteService : ICardWriteService
    {
        public CardWriteService(ICardWriter cardWriter)
        {
            CardWriter = cardWriter;
        }

        private readonly ICardWriter CardWriter;

        public string Write()
        {
            return CardWriter.Write();
        }
    }
}