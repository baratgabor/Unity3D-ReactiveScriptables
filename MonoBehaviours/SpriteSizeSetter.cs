using UnityEngine;

namespace LeakyAbstraction.ReactiveScriptables
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteSizeSetter : MonoBehaviour
    {
        [SerializeField]
        private BoundsProperty _cameraBounds = default;
        private SpriteRenderer _spriteRenderer;

        private void Awake()
        {
            if (_cameraBounds == null)
                throw new System.Exception("Dependency not set.");

            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            ApplyBounds(_cameraBounds.Get());
        }

        private void OnEnable()
            => _cameraBounds.Event += ApplyBounds;

        private void OnDisable()
            => _cameraBounds.Event -= ApplyBounds;

        private void ApplyBounds(Bounds bounds)
        {
            Debug.Log("SpriteSizeSetter ran.");

            var spriteBounds = _spriteRenderer.bounds;
            var spriteWidth = spriteBounds.size.x;
            var spriteHeight = spriteBounds.size.y;

            var spriteScale = transform.localScale;
            float unitWidth = spriteWidth / spriteScale.x;
            float unitHeight = spriteHeight / spriteScale.y;

            float newScaleX = bounds.size.x / unitWidth * 1.1f;
            float newScaleY = bounds.size.y / unitHeight * 1.1f;

            transform.localScale = new Vector3(newScaleX, newScaleY);
        }
    }
}