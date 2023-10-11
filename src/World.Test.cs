using Xunit;
using LibWorld;
using LibRay;
using LibColor;
using LibLight;
using LibForm;
using LibTuple;
using LibMaterial;
using LibComparinator;
using LibIntersection;
using LibMatrix;
using LibPattern;

namespace LibWorld.Test;

public class WorldTest {
    DefaultWorld varDefaultWorld = new DefaultWorld();
    [Fact]
    public void CanaryTest() {
        Assert.Equal(1, 1);
        Assert.True(true);
    }
    [Fact]
    public void WorldEmptyCtor() {
        World w = new World();
        Assert.Empty(w._fieldObjects);
        Assert.Empty(w._fieldLights);
    }
    [Fact]
    public void WorldDefaultCtor() {
        PointSource l = new PointSource(new Point(-10,10,-10), new Color(1,1,1));
        Sphere s = new Sphere();
        s.SetMaterial(new Material());
        s._fieldMaterial.mbrColor = new Color(0.8,1.0,0.6);
        s._fieldMaterial.mbrDiffuse = 0.7;
        s._fieldMaterial.mbrSpecular = 0.2;
        Sphere t = new Sphere();
        // t.setTransform(*(ScalingMatrix(0.5,0.5,0.5) * IdentityMatrix(4,4)));
        t.SetTransform(new ScalingMatrix(0.5,0.5,0.5) * new IdentityMatrix(4,4));
        Assert.True(l.CheckEqual(varDefaultWorld._fieldLights[0]));
        Assert.True(varDefaultWorld._fieldObjects[0].CheckEqual(s));
        Assert.True(varDefaultWorld._fieldObjects[1].CheckEqual(t));
    }
    [Fact]
    public void WorldRayIntersect() {
        Ray r = new Ray(new Point(0,0,-5), new Vector(0,0,1));
        Intersections xs = varDefaultWorld.GetIntersect(r);
        // Assert.Equal(xs.mbrIntersections.Count(), 4);
        // Assert.Equal(xs.mbrIntersections[0].mbrTime, 4);
        // Assert.Equal(xs.mbrIntersections[1].mbrTime, 4.5);
        // Assert.Equal(xs.mbrIntersections[2].mbrTime, 5.5);
        // Assert.Equal(xs.mbrIntersections[3].mbrTime, 6);
        Assert.Equal(4, xs._fieldIntersections.Count());
        Assert.Equal(4, xs._fieldIntersections[0]._fieldTime);
        Assert.Equal(4.5, xs._fieldIntersections[1]._fieldTime);
        Assert.Equal(5.5, xs._fieldIntersections[2]._fieldTime);
        Assert.Equal(6, xs._fieldIntersections[3]._fieldTime);
    }
    [Fact]
    public void WorldIntersectionShading() {
        Ray r = new Ray(new Point(0,0,-5), new Vector(0,0,1));
        Form obj = varDefaultWorld._fieldObjects[0];
        Sphere s = new Sphere();
        s.SetMaterial(new Material());
        s._fieldMaterial.mbrColor = new Color(0.8,1.0,0.6);
        s._fieldMaterial.mbrDiffuse = 0.7;
        s._fieldMaterial.mbrSpecular = 0.2;
        Assert.True(obj.CheckEqual(s));
        // Intersection i = Intersection(4,std::make_unique<Form>(obj));
        // Intersection i = Intersection(4,new Form(obj));
        Intersections varIx = new Intersections(4,new Form(obj));
        // Assert.True(varIx.mbrObject.CheckEqual(obj));
        IntersectionState varIs = varIx.getHit().GetState(r, varIx._fieldIntersections);
        Color c = varDefaultWorld.GetColorShaded(varIs);
        Color expectedColor = new Color(0.38066, 0.47583, 0.2855);
        Assert.True(c.CheckEqual(expectedColor));
    }
    [Fact]
    public void WorldIntersectionInteriorShading() {
        varDefaultWorld._fieldLights[0] = new PointSource(new Point(0,0.25,0), new Color(1,1,1));
        Ray r = new Ray(new Point(0,0,0), new Vector(0,0,1));
        Form obj = varDefaultWorld._fieldObjects[1];
        // Intersection i = Intersection(0.5,std::make_unique<Form>(obj));
        // Intersection i = Intersection(0.5,new Form(obj));
        Intersections varIx = new Intersections(0.5,new Form(obj));
        IntersectionState varIs = varIx.getHit().GetState(r, varIx._fieldIntersections);
        // Color c = varDefaultWorld.getColorShaded(varIs);
        Color c = varDefaultWorld.GetColorLighting(varIs);
        Color expectedColor = new Color(0.90498, 0.90498, 0.90498);
        Assert.True(c.CheckEqual(expectedColor));
    }
    [Fact]
    public void WorldColorMvarIssvarIsBlack() {
        Ray r = new Ray(new Point(0,0,-5), new Vector(0,1,0));
        Color c = varDefaultWorld.GetColor(r);
        Color expectedColor = new Color(0,0,0);
        Assert.True(c.CheckEqual(expectedColor));
    }
    [Fact]
    public void WorldColorHit() {
        Ray r = new Ray(new Point(0,0,-5), new Vector(0,0,1));
        Color c = varDefaultWorld.GetColor(r);
        Color expectedColor = new Color(0.38066, 0.47583, 0.2855);
        Assert.True(c.CheckEqual(expectedColor));
    }
    [Fact]
    public void WorldColorHitInsideInnerSphere() {
        varDefaultWorld._fieldObjects[0]._fieldMaterial.mbrAmbient = 1;
        varDefaultWorld._fieldObjects[1]._fieldMaterial.mbrAmbient = 1;
        Ray r = new Ray(new Point(0,0,0.75), new Vector(0,0,-1));
        Color c = varDefaultWorld.GetColor(r);
        Color expectedColor = varDefaultWorld._fieldObjects[1]._fieldMaterial.mbrColor;
        Assert.True(c.CheckEqual(expectedColor));
    }
    [Fact]
    public void NoShadowIfObjectCollinearWithPointLight() {
        Point varPoint= new Point(0,10,0);
        Assert.False(varDefaultWorld.CheckShadowed(varPoint));
    }
    [Fact]
    public void ShadowIfObjectBetweenPointLight() {
        Point varPoint= new Point(10,-10,10);
        Assert.True(varDefaultWorld.CheckShadowed(varPoint));
    }
    [Fact]
    public void NoShadowIfObjectBehindLight() {
        Point varPoint= new Point(-20,20,-20);
        Assert.False(varDefaultWorld.CheckShadowed(varPoint));
    }
    [Fact]
    public void NoShadowIfObjectBehindPoint() {
        Point varPoint= new Point(-2,2,-2);
        Assert.False(varDefaultWorld.CheckShadowed(varPoint));
    }
    [Fact]
    public void ShadeWithShadowIntersections() {
        World varWorld = new World();
        PointSource varLight = new PointSource(new Point(0,0,-10), new Color(1,1,1));
        varWorld.SetLight(varLight);
        Sphere varS1 = new Sphere();
        varWorld.SetObject(varS1);
        // varWorld.setObject(std::make_unique<Sphere>(varS1));
        Sphere varS2 = new Sphere();
        varS2.SetTransform(new TranslationMatrix(0,0,10));
        varWorld.SetObject(varS2);
        // varWorld.setObject(std::make_unique<Sphere>(varS2));
        Ray varRay = new Ray(new Point(0,0,5), new Vector(0,0,1));
        // Intersection varIx = Intersection(4,std::make_unique<Sphere>(varS2));
        // Intersection varIx = Intersection(4,new Sphere(varS2));
        Intersections varIx = new Intersections(4, varS2);
        IntersectionState varvarIs = varIx.getHit().GetState(varRay, varIx._fieldIntersections);
        Color varClr = varWorld.GetColorShaded(varvarIs);
        Color varExpectedClr = new Color(0.1,0.1,0.1);
        Assert.True(varExpectedClr.CheckEqual(varClr));
    }
    [Fact]
    public void NonReflectiveReflectedColor() {
        DefaultWorld varWorld = new DefaultWorld();
        Ray varRay = new Ray(new Point(0,0,0), new Vector(0,0,1));
        Form varObj = varWorld._fieldObjects[1];
        varObj._fieldMaterial.mbrAmbient = 1;
        // Intersection varIx = Intersection(1, varObj);
        Intersections varIx = new Intersections(1, varObj);
        IntersectionState varvarIs = varIx.getHit().GetState(varRay, varIx._fieldIntersections);
        Color varColor = varWorld.GetColorReflect(varvarIs);
        Assert.True(varColor.CheckEqual(new Color(0,0,0)));
    }
    [Fact]
    public void ReflectiveReflectedColor() {
        DefaultWorld varWorld = new DefaultWorld();
        Plane varPlane = new Plane();
        varPlane._fieldMaterial.mbrReflective = 0.5;
        varPlane.SetTransform(new TranslationMatrix(0,-1,0));
        varWorld.SetObject(varPlane);
        Ray varRay = new Ray(new Point(0,0,-3), new Vector(0,-Math.Sqrt(2)/2,Math.Sqrt(2)/2));
        // Intersection varIx = Intersection(Math.Sqrt(2), varPlane);
        Intersections varIx = new Intersections(Math.Sqrt(2), varPlane);
        IntersectionState varvarIs = varIx.getHit().GetState(varRay, varIx._fieldIntersections);
        Color varColor = varWorld.GetColorReflect(varvarIs);
        Assert.True(varColor.CheckEqual(new Color(0.19032,0.2379,0.14274)));
        // Color varExpectedColor = new Color(0.19033,0.23791, 0.14274);
        // Assert.True(varExpectedColor.CheckEqual(varColor));
    }
    [Fact]
    public void ReflectiveShading() {
        DefaultWorld varWorld = new DefaultWorld();
        Plane varPlane = new Plane();
        varPlane._fieldMaterial.mbrReflective = 0.5;
        varPlane.SetTransform(new TranslationMatrix(0,-1,0));
        varWorld.SetObject(varPlane);
        Ray varRay = new Ray(new Point(0,0,-3), new Vector(0,-Math.Sqrt(2)/2,Math.Sqrt(2)/2));
        // Intersection varIx = Intersection(Math.Sqrt(2), varPlane);
        Intersections varIx = new Intersections(Math.Sqrt(2), varPlane);
        IntersectionState varvarIs = varIx.getHit().GetState(varRay, varIx._fieldIntersections);
        // std::cout << "expect . t=1.41421 c(.6864,.6864,.6864) + r(.1903, .2379, .1428)" << std::endl;
        Color varColor = varWorld.GetColorShaded(varvarIs);
        Assert.True(varColor.CheckEqual(new Color(0.87677, 0.92436, 0.82918)));
    }
    [Fact]
    public void InfiniteReflectionsBetweenMirrors() {
        World varWorld = new World();
        varWorld.SetLight(new PointSource(new Point(0,0,0), new Color(1,1,1)));
        Plane varLower = new Plane();
        varLower._fieldMaterial.mbrReflective = 1;
        varLower.SetTransform(new TranslationMatrix(0,-1,0));
        varWorld.SetObject(varLower);
        Plane varUpper = new Plane();
        varUpper._fieldMaterial.mbrReflective = 1;
        varUpper.SetTransform(new TranslationMatrix(0,1,0));
        varWorld.SetObject(varUpper);
        Ray varRay = new Ray(new Point(0,0,0), new Vector(0,1,0));
        Color varColor = varWorld.GetColor(varRay);
        // EXPECT_NO_FATAL_FAILURE();
    }
    [Fact]
    public void InfiniteReflectionsAtMaxDepth() {
        DefaultWorld varWorld = new DefaultWorld();
        Plane varPlane = new Plane();
        varPlane._fieldMaterial.mbrReflective = 0.5;
        varPlane.SetTransform(new TranslationMatrix(0,-1,0));
        varWorld.SetObject(varPlane);
        Ray varRay = new Ray(new Point(0,0,-3), new Vector(0,-Math.Sqrt(2)/2,Math.Sqrt(2)/2));
        Intersections varIx = new Intersections(Math.Sqrt(2), varPlane);
        // Intersection varIx = Intersection(Math.Sqrt(2), varPlane);
        IntersectionState varvarIs = varIx.getHit().GetState(varRay, varIx._fieldIntersections);
        Color varColor = varWorld.GetColorReflect(varvarIs, 0);
        Assert.True(varColor.CheckEqual(new Color(0,0,0)));
    }
    [Fact]
    public void RefractiveWithOpaquevarIsBlack() {
        DefaultWorld varWorld = new DefaultWorld();
        Form varObj = varWorld._fieldObjects[0];
        Ray varRay = new Ray(new Point(0,0,-5), new Vector(0,0,1));
        Intersections varIx = new Intersections();
        varIx.setIntersect(4, varObj);
        varIx.setIntersect(4, varObj);
        IntersectionState varvarIs = varIx._fieldIntersections[0].GetState(varRay, varIx._fieldIntersections);
        Color varColor = varWorld.GetColorRefracted(varvarIs, 5);
        Assert.True(varColor.CheckEqual(new Color(0,0,0)));
    }
    [Fact]
    public void InfiniteRefractionvarIsBlack() {
        DefaultWorld varWorld = new DefaultWorld();
        Form varObj = varWorld._fieldObjects[0];
        varObj._fieldMaterial.mbrTransparency = 1.0;
        varObj._fieldMaterial.mbrTransparency = 1.5;
        Ray varRay = new Ray(new Point(0,0,-5), new Vector(0,0,1));
        Intersections varIx = new Intersections();
        varIx.setIntersect(4, varObj);
        varIx.setIntersect(4, varObj);
        IntersectionState varvarIs = varIx._fieldIntersections[0].GetState(varRay, varIx._fieldIntersections);
        Color varColor = varWorld.GetColorRefracted(varvarIs, 0);
        Assert.True(varColor.CheckEqual(new Color(0,0,0)));
    }
    [Fact]
    public void TotalInternalRefractionvarIsBlack() {
        DefaultWorld varWorld = new DefaultWorld();
        Form varObj = varWorld._fieldObjects[0];
        varObj._fieldMaterial.mbrTransparency = 1.0;
        varObj._fieldMaterial.mbrRefractiveIndex = 1.5;
        Ray varRay = new Ray(new Point(0,0,Math.Sqrt(2)/2), new Vector(0,1,0));
        Intersections varIx = new Intersections();
        varIx.setIntersect(-Math.Sqrt(2)/2, varObj);
        varIx.setIntersect(Math.Sqrt(2)/2, varObj);
        IntersectionState varvarIs = varIx._fieldIntersections[1].GetState(varRay, varIx._fieldIntersections);
        Color varColor = varWorld.GetColorRefracted(varvarIs, 5);
        Assert.True(varColor.CheckEqual(new Color(0,0,0)));
    }
    [Fact]
    public void RefractionBasic() {
        DefaultWorld varWorld = new DefaultWorld();
        varWorld._fieldObjects[0]._fieldMaterial.mbrAmbient = 1.0;
        // varWorld.mbrObjects[0].mbrMaterial.setPattern(new Pattern(new Color(0,0,0), new Color(1,1,1)));
        varWorld._fieldObjects[0]._fieldMaterial.SetPattern(new Pattern(new Color(0,0,0), new Color(1,1,1)));
        varWorld._fieldObjects[0]._fieldMaterial.mbrPattern.GetColorLocal(new Point(0,0,0));
        varWorld._fieldObjects[1]._fieldMaterial.mbrTransparency = 1.0;
        varWorld._fieldObjects[1]._fieldMaterial.mbrRefractiveIndex = 1.5;
        Ray varRay = new Ray(new Point(0,0,0.1), new Vector(0,1,0));
        Intersections varIx = new Intersections();
        varIx.setIntersect(-0.9899, varWorld._fieldObjects[0]);
        varIx.setIntersect(-0.4899, varWorld._fieldObjects[1]);
        varIx.setIntersect(0.4899, varWorld._fieldObjects[1]);
        varIx.setIntersect(0.9899, varWorld._fieldObjects[0]);
        IntersectionState varvarIs = varIx._fieldIntersections[2].GetState(varRay, varIx._fieldIntersections);
        Color varColor = varWorld.GetColorRefracted(varvarIs, 5);
        // World::getColorRefracted()::varNToN = n1/n2 = 1.5/1 = 1.5
        // World::getColorRefracted()::varCosThetaI = (0,-1,0,0)dot(0,-0.979796, -0.19999,0) = 0.979796073
        // World::getColorRefracted()::varSinThetaTSquared = 1.5^2 * (1-0.979796^2) = 0.08999
        // World::getColorRefracted()::varCosThetaT = Math.Sqrt(1-0.08999) = 0.9539
        // World::getColorRefracted()::varRefractDirection = (0,-0.979796, -0.19999,0) * (1.5*0.979796-0.9539) - (0,-1,0,0) * 1.5 = (0,0.99466,-0.1031,0)
        // World::getColorRefracted()::varRefractRay = ((0,0.4899,0.10000,0),(0,0.99466,-0.1031,0))
        // World::getColorRefracted()::getcolor() * mbrTransparency(1)
        // World::getcolor()::hit::mbrTime=(0.511)
        // World::getcolor()::hit::mbrObject::mbrColor=(0.8000,1,0.59999)
        // World::getcolor()::hit::mbrPoint=(0,.9988,0.047219,1)
        // World::getcolor()::hit::mbrOverPoint=(0,0.9987,0.0472,1)
        // World::getcolor()::hit::mbrUnderPoint=(0,0.99889,0.04721,1)
        // World::getcolor()::hit::mbrEye=(0,-.9946,0.1031,0)
        // World::getcolor()::hit::mbrNormal=(0,-0.99888,-.047219,0)
        // World::getcolor()::hit::mbrReflect=(0,-0.9804,-.1965,0)
        // World::getcolor()::hit::mbrRefractiveIndexOne=(1)
        // World::getcolor()::hit::mbrRefractiveIndexTwo=(1)
        // World::getcolorShaded()::argIxState.mbrOverPoint=(0,0.9988,0.0472,1)
        // World::getcolorShaded()::mbrRefractiveIndexTwo=(0)varInShadow=(true)
        // Material::getColor(...)::varColor=mbrColor=(0,0.998885,0.472194
        // Material::getColor(...)::varShade=(0,0.998885,0.472194
        // Material::getColor(...)::varResAmbient=(0,0.998885,0.472194
        // Form::getColor(...)::varRes=(0,0.998885,0.472194
        // World::getColorShaded()::varDiffuse=(0,0.998885,0.472194
        // World::getColorShaded()::varReflect=(0,0,0)
        // World::getColorShaded()::varRes=(0,0.998885,0.472194
        Assert.True(varColor.CheckEqual(new Color(0, 0.99888, 0.04725)));
    }
    [Fact]
    public void RefractionTransparentColor() {
        DefaultWorld varWorld = new DefaultWorld();
        Plane varFloor = new Plane();
        varFloor.SetTransform(new TranslationMatrix(0,-1,0));
        varFloor._fieldMaterial.mbrTransparency = 0.5;
        varFloor._fieldMaterial.mbrRefractiveIndex = 1.5;
        varWorld.SetObject(varFloor);
        Sphere varSphere = new Sphere();
        varSphere.SetTransform(new TranslationMatrix(0,-3.5,-.5));
        varSphere._fieldMaterial.mbrColor=new Color(1,0,0);
        varSphere._fieldMaterial.mbrAmbient = 0.5;
        varWorld.SetObject(varSphere);
        Ray varRay = new Ray(new Point(0,0,-3), new Vector(0,-Math.Sqrt(2)/2,Math.Sqrt(2)/2));
        Intersections varIx = new Intersections(Math.Sqrt(2), varFloor);
        IntersectionState varvarIs = varIx._fieldIntersections[0].GetState(varRay, varIx._fieldIntersections);
        Color varColor = varWorld.GetColorShaded(varvarIs, 5);
        Assert.True(varColor.CheckEqual(new Color(0.93642, 0.68642, 0.68642)));
    }
}