'use strict';

$(document).ready(function () { 

    let hull = new DynamicConvexHull();
    let all_points = [];

    $("#canvas").on("click", function(e) {
        let to_add = new Point(e.pageX, -e.pageY);
        let found = false;

        all_points.forEach(point => {
            if (Math.sqrt((point.x - to_add.x) * (point.x - to_add.x) + (point.y - to_add.y) * (point.y - to_add.y)) <= 5) {                

                hull.delete_point(point);

                point.x = -100;
                point.y = -100;

                found = true;
            }
        });

        if (!found) {
            hull.insert_point(to_add);
            all_points.push(to_add);
        }

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


