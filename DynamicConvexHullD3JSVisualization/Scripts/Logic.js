'use strict';

$(document).ready(function () { 

    let hull = new DynamicConvexHull();
    let all_points = [];

    $("#canvas").on("click", function(e) {
        hull.insert_point(new Point(e.pageX, -e.pageY));
        all_points.push(new Point(e.pageX, -e.pageY));

        //alert("add " + (e.pageX ) +" "+ (-e.pageY ));
        

        let canvas = document.getElementById("canvas");
        let ctx = canvas.getContext("2d");
        ctx.clearRect(0, 0, 500, 500);

        let list = hull.get_hull_as_array();
        ctx.beginPath();
        ctx.moveTo(list[list.length - 1].x, -list[list.length - 1].y);
        list.forEach(point => {
            ctx.lineTo(point.x, -point.y);
            ctx.moveTo(point.x, -point.y);
        });
        ctx.stroke();

        all_points.forEach(point => {
            ctx.beginPath();
            ctx.arc(point.x, -point.y, 5, 0, 2*Math.PI);
            ctx.stroke();
        });
    });

 });


