import { useEffect, useRef } from "react";

export type ScrollInfo = { offset: number; atStart: boolean; atEnd: boolean };

export const useHorizontalScroll = (onScroll?: (data: ScrollInfo) => void) => {
  const elRef = useRef<HTMLDivElement | null>(null);
  useEffect(() => {
    const el = elRef.current;
    if (el) {
      const onWheel = (e: WheelEvent) => {
        if (e.deltaY == 0) return;
        e.preventDefault();
        const left = el.scrollLeft + e.deltaY;
        el.scrollTo({
          left,
          behavior: "smooth",
        });
        onScroll &&
          onScroll({
            offset: left,
            atStart: left <= 0,
            atEnd: left >= el.scrollWidth - el.offsetWidth,
          });
      };
      el.addEventListener("wheel", onWheel, { passive: false });
      onScroll &&
        onScroll({
          offset: el.scrollLeft,
          atStart: el.scrollLeft <= 0,
          atEnd: el.scrollLeft >= el.scrollWidth - el.offsetWidth,
        });
      return () => el.removeEventListener("wheel", onWheel);
    }
  }, [onScroll]);
  return elRef;
};
