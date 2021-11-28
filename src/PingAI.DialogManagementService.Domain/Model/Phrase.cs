using System;
using System.Collections;
using System.Collections.Generic;

namespace PingAI.DialogManagementService.Domain.Model
{
    public class Phrase : IEnumerable<PhrasePart>
    {
        private int _currentPosition;
        private readonly List<PhrasePart> _parts;
        private readonly int _displayOrder;

        public Guid PhraseId { get; }

        public Phrase(int displayOrder)
        {
            PhraseId = Guid.NewGuid();
            _currentPosition = 0;
            _parts = new List<PhrasePart>();
            _displayOrder = displayOrder;
        }

        public Phrase AppendText(string text)
        {
            _parts.Add(new PhrasePart(PhraseId, _currentPosition++, text,
                null, PhrasePartType.TEXT, null, null, _displayOrder));
            return this;
        }

        public Phrase AppendEntity(string text, EntityName entityName)
        {
            _parts.Add(new PhrasePart(PhraseId, _currentPosition++, text,
                null, PhrasePartType.ENTITY, entityName, null, _displayOrder));
            return this;
        }

        public Phrase AppendConstantEntity(string value, EntityName entityName)
        {
            _parts.Add(new PhrasePart(PhraseId, _currentPosition++, null,
                value, PhrasePartType.CONSTANT_ENTITY, entityName, null, _displayOrder));
            return this; 
        }

        public Phrase AppendParts(IEnumerable<PhrasePart> parts)
        {
            foreach (var part in parts ?? throw new ArgumentNullException(nameof(parts)))
            {
                _parts.Add(part ?? throw new InvalidOperationException("part cannot be null."));
            }
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
