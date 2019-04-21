using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeriousOrganizerGui.Data
{
    public class LensSingle<T> : Lens<T, T>
    {
        public LensSingle(BetterObservable<T> source) : base(source, x => x)
        {
        }
    }


    public class Lens<T, S>
    {

        public Lens(BetterObservable<T> source, Func<T, S> transform)
        {
            _source = source;
            _transform = transform;

            _source.CollectionChanged += SourceChanged;
            Update();
        }

        private Func<T, S> _transform;
        private Func<T, bool> _filter = e => true;

        private BetterObservable<T> _source;
        private BetterObservable<S> _sink = new BetterObservable<S>();

        public BetterObservable<S> GetSink => _sink;

        public void AddFilter(Func<T, bool> func)
        {
            _filter = func;
        }

        private void SourceChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Update();            
        }

        private void Update()
        {
            _sink.Replace(_source.Where(_filter).Select(_transform));
        }
    }
}
