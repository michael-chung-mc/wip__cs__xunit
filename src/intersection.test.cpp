#include "intersection.h"
#include "form.h"
#include "comparinator.h"
#include "ray.h"
#include "pch.h"

class IntersectionTest : public ::testing::Test {
public:
	Comparinator varComp;
	void SetUp () override {
		varComp = Comparinator();
	}
};

TEST_F(IntersectionTest, CanaryTest) {
	EXPECT_EQ(1, 1);
	EXPECT_TRUE(true);
};

TEST_F(IntersectionTest, InitTest) {
	Sphere s = Sphere();
	double t = 3.5;
	Intersection i = Intersection(t, s);
	EXPECT_EQ(i.time, t);
	EXPECT_TRUE(i.object.checkEqual(s));
};

TEST_F(IntersectionTest, AggregationTest) {
	Sphere s = Sphere();
	double t1 = 1;
	double t2 = 2;
	Intersections i = Intersections(t1, s);
	i.intersect(t2,s);
	std::vector<Intersection> xs = i.intersections;
	EXPECT_EQ(xs.size(),2);
	EXPECT_EQ(xs[0].time, t1);
	EXPECT_EQ(xs[1].time, t2);
};

TEST_F(IntersectionTest, PositiveT) {
	Sphere s = Sphere();
	Intersections i = Intersections(1,s);
	i.intersect(2, s);
	std::vector<Intersection> xs = i.intersections;
	Intersection hit = i.hit();
	EXPECT_TRUE(hit.checkEqual(xs[0]));
};

TEST_F(IntersectionTest, NegativeT) {
	Sphere s = Sphere();
	Intersections i = Intersections(-1,s);
	i.intersect(2, s);
	std::vector<Intersection> xs = i.intersections;
	Intersection hit = i.hit();
	EXPECT_TRUE(hit.checkEqual(xs[1]));
};

TEST_F(IntersectionTest, AllNegativeT) {
	Sphere s = Sphere();
	Intersections i = Intersections(-1,s);
	i.intersect(-2, s);
	std::vector<Intersection> xs = i.intersections;
	Intersection hit = i.hit();
	EXPECT_TRUE(hit.checkEqual(Intersection()));
};

TEST_F(IntersectionTest, HitIsNonnegativeIntersection) {
	Sphere s = Sphere();
	Intersections i = Intersections(5,s);
	i.intersect(7, s);
	i.intersect(-3, s);
	i.intersect(2, s);
	std::vector<Intersection> xs = i.intersections;
	Intersection hit = i.hit();
	EXPECT_TRUE(hit.checkEqual(xs[1]));
};

TEST_F(IntersectionTest, PrecomputeIntersectionState) {
	Ray r = Ray(Point(0,0,-5), Vector(0,0,1));
	Sphere s = Sphere();
	Intersection i = Intersection(4, s);
	IntersectionState is = i.getState(r);
	EXPECT_TRUE(varComp.checkFloat(is.time, i.time));
	EXPECT_TRUE(is.object.checkEqual(i.object));
	EXPECT_TRUE(varComp.checkTuple(is.point, Point(0,0,-1)));
	EXPECT_TRUE(varComp.checkTuple(is.pov, Vector(0,0,-1)));
	EXPECT_TRUE(varComp.checkTuple(is.normal, Vector(0,0,-1)));
};

TEST_F(IntersectionTest, PrecomputeIntersectionStateInteriorHitFalse) {
	Ray r = Ray(Point(0,0,-5), Vector(0,0,1));
	Sphere s = Sphere();
	Intersection i = Intersection(4, s);
	IntersectionState is = i.getState(r);
	EXPECT_FALSE(is.inside);
}

TEST_F(IntersectionTest, PrecomputeIntersectionStateInteriorHitTrue) {
	Ray r = Ray(Point(0,0,0), Vector(0,0,1));
	Sphere s = Sphere();
	Intersection i = Intersection(1, s);
	IntersectionState is = i.getState(r);
	EXPECT_TRUE(varComp.checkTuple(is.point, Point(0,0,1)));
	EXPECT_TRUE(varComp.checkTuple(is.pov, Vector(0,0,-1)));
	EXPECT_TRUE(is.inside);
	EXPECT_TRUE(varComp.checkTuple(is.normal, Vector(0,0,-1)));
}

TEST_F(IntersectionTest, HitShouldOffsetPoint) {
	Ray varRay = Ray(Point(0,0,-5), Vector(0,0,1));
	Sphere varSphere = Sphere();
	varSphere.setTransform(TranslationMatrix(0,0,1));
	Intersection varIntersection = Intersection(5, varSphere);
	IntersectionState varIs = varIntersection.getState(varRay);
	EXPECT_TRUE(varIs.mbrOverPoint.z < -getEpsilon()/2);
	EXPECT_TRUE(varIs.point.z > varIs.mbrOverPoint.z);
}