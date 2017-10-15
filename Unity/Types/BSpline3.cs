using UnityEngine;

namespace Hull.Unity.Types {
    public class BSpline3 {
        private float _lenght;
        private Vector3 _begin;
        private Vector3 _controlPointA;
        private Vector3 _controlPointB;
        private Vector3 _end;

        public BSpline3(Vector3 begin, Vector3 controlPointA, Vector3 controlPointB, Vector3 end) {
            _begin = begin;
            _controlPointA = controlPointA;
            _controlPointB = controlPointB;
            _end = end;
            _lenght = float.NaN;
        }

        public Vector3 Begin {
            get { return _begin; }
            set {
                _begin = value;
                _lenght = float.NaN;
            }
        }

        public Vector3 ControlPointA {
            get { return _controlPointA; }
            set {
                _controlPointA = value;
                _lenght = float.NaN;
            }
        }
        
        public Vector3 ControlPointB {
            get { return _controlPointB; }
            set {
                _controlPointB = value;
                _lenght = float.NaN;
            }
        }

        public Vector3 End {
            get { return _end; }
            set {
                _end = value;
                _lenght = float.NaN;
            }
        }

        public float Length {
            get {
                if (float.IsNaN(_lenght)) {
                    var step = .1f;
                    var x = step;
                    Vector3 p = Begin, pp;
                    _lenght = 0;

                    for (var i = 0; i < 10; i++) {
                        pp = GetPoint(x);
                        _lenght += (pp - p).magnitude;
                        p = pp;
                        x += step;
                    }
                }

                return _lenght;
            }
        }

        public Vector3 GetPoint(float u) {
            var ru = 1 - u;
            return Begin * ru * ru * ru + 
                   ControlPointA * 3 * u * ru * ru + 
                   ControlPointB * 3 * u * u * ru + 
                   End * u * u * u;
        }

        public Vector3 GetForward(float u) {
            var epsilon = 0.025f;
            if (u < epsilon) {
                return ControlPointA - Begin;
            }
            if (u >= 1 - epsilon) {
                return End - ControlPointB;
            }

            return GetPoint(u + epsilon) - GetPoint(u - epsilon);
        }

        public Vector3 GetNormal(float u) {
            return Vector3.Cross(GetForward(u), Vector3.up);
        }

        public bool IsPointOnCurve(Vector3 point, float tolerance) {
            var len = Length;
            tolerance = tolerance * tolerance;
            const float stepSize = 1;
            var delta = 1 / (len / stepSize);
            var dist = (_begin - _end).sqrMagnitude + tolerance;

            if ((point - ((_begin + _end) / 2)).sqrMagnitude > dist) {
                return false;
            }

            var t = 0f;
            while (t <= 1) {
                dist = (point - GetPoint(t)).sqrMagnitude;
                if (dist < tolerance) {
                    return true;
                }

                t += delta;
            }

            return false;
        }

        public BSpline3 Reversed {
            get { return new BSpline3(End, ControlPointB, ControlPointA, Begin); }
        }

        public override bool Equals(object obj) {
            if (obj == null) {
                return false;
            }

            var other = (BSpline3)obj;

            return _begin.Equals(other._begin) &&
                   _controlPointA.Equals(other._controlPointA) &&
                   _controlPointB.Equals(other._controlPointB) &&
                   _end.Equals(other._end);
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }
    }
}
