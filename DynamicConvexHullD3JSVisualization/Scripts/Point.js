'use strict';

var PointPosition = {
    Left: "Left",
    Right: "Right",
    On: "On"
}

class Point {

    constructor(x, y) {
        this.x = x;
        this.y = y;
    }

    compare_to(other_point) {
        if (this.y < other_point.y || (this.y == other_point.y && this.x < other_point.x)) {
            return -1;
        } 
        if (this.y > other_point.y || (this.y == other_point.y && this.x > other_point.x)) {
            return 1;
        }
        return 0;
    }
}