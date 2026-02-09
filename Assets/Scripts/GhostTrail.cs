using UnityEngine;
using DG.Tweening;

public class GhostTrail : MonoBehaviour
{
    private Movement move;
    private AnimationScript anim;
    private SpriteRenderer sr;

    public Transform ghostsParent;
    public Color trailColor;
    public Color fadeColor;
    public float ghostInterval;
    public float fadeTime;

    private void Start()
    {
        anim = FindFirstObjectByType<AnimationScript>();
        move = FindFirstObjectByType<Movement>();
        sr = GetComponent<SpriteRenderer>();
    }

    public void ShowGhost()
    {
        if (ghostsParent == null) return;

        Sequence s = DOTween.Sequence();

        for (int i = 0; i < ghostsParent.childCount; i++)
        {
            Transform currentGhost = ghostsParent.GetChild(i);

            // Copiamos posicion y sprite del jugador actual
            s.AppendCallback(() => currentGhost.position = move.transform.position);
            s.AppendCallback(() => currentGhost.GetComponent<SpriteRenderer>().flipX = anim.sr.flipX);
            s.AppendCallback(() => currentGhost.GetComponent<SpriteRenderer>().sprite = anim.sr.sprite);

            s.Append(currentGhost.GetComponent<SpriteRenderer>().DOColor(trailColor, 0));

            s.AppendCallback(() => FadeSprite(currentGhost));
            s.AppendInterval(ghostInterval);
        }
    }

    public void FadeSprite(Transform current)
    {
        current.GetComponent<SpriteRenderer>().DOKill();
        current.GetComponent<SpriteRenderer>().DOColor(fadeColor, fadeTime);
    }
}