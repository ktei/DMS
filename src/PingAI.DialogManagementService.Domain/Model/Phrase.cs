using System;
using System.Collections;
using System.Collections.Generic;

namespace PingAI.DialogManagementService.Domain.Model
{
    public class Phrase : IEnumerable<PhrasePart>
    {
        private readonly Guid _phraseId;
        private int _currentPosition;
        private readonly List<PhrasePart> _parts;
        private readonly int _displayOrder;

        public Phrase(int displayOrder)
        {
            _phraseId = Guid.NewGuid();
            _currentPosition = 0;
            _parts = new List<PhrasePart>();
            _displayOrder = displayOrder;
        }

        public Phrase AppendText(string text)
        {
            _parts.Add(new PhrasePart(_phraseId, _currentPosition++, text,
                null, PhrasePartType.TEXT, null, null, _displayOrder));
            return this;
        }

        public Phrase AppendEntity(string text, EntityName entityName)
        {
            _parts.Add(new PhrasePart(_phraseId, _currentPosition++, text,
                null, PhrasePartType.ENTITY, entityName, null, _displayOrder));
            return this;
        }

        public Phrase AppendConstantEntity(string value, EntityName entityName)
        {
            _parts.Add(new PhrasePart(_phraseId, _currentPosition++, null,
                value, PhrasePartType.CONSTANT_ENTITY, entityName, null, _displayOrder));
            return this; 
        }

        public IReadOnlyList<PhrasePart> Build() => _parts.AsReadOnly();
        public IEnumerator<PhrasePart> GetEnumerator()
        {
            return _parts.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
