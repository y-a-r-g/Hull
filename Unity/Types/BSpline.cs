using UnityEngine;

namespace Hull.Unity.Types {
    public class BSpline {
        private float _lenght;
        private Vector3 _begin;
        private Vector3 _controlPoint;
        private Vector3 _end;

        public BSpline(Vector3 begin, Vector3 controlPoint, Vector3 end) {
            _begin = begin;
            _controlPoint = controlPoint;
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

        public Vector3 ControlPoint {
            get { return _controlPoint; }
            set {
                _controlPoint = value;
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
            return Begin * (1 - u) * (1 - u) + ControlPoint * (1 - u) * u * 2 + End * u * u;
        }

        public Vector3 GetForward(float u) {
            var epsilon = 0.025f;
            if (u < epsilon) {
                return ControlPoint - Begin;
            }
            if (u >= 1 - epsilon) {
                return End - ControlPoint;
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

        public BSpline Reversed {
            get { return new BSpline(End, ControlPoint, Begin); }
        }

        public override bool Equals(object obj) {
            if (obj == null) {
                return false;
            }

            var other = (BSpline)obj;

            return _begin.Equals(other._begin) &&
                _controlPoint.Equals(other._controlPoint) &&
                _end.Equals(other._end);
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }
    }
}
