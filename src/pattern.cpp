#include "pattern.h"
#include "color.h"
#include "tuple.h"
#include "matrix.h"
#include <cmath>
#include <memory>

Pattern::Pattern()
{
    mbrWhite = Color(1,1,1);
    mbrBlack = Color(0,0,0);
    mbrTransform = std::make_unique<Matrix>(IdentityMatrix(4,4));
}
Pattern::Pattern(const Pattern& other)
{
    mbrColors = other.mbrColors;
    mbrTransform = std::make_unique<Matrix>(*other.mbrTransform.get());
}
Pattern::~Pattern() { }
Color Pattern::getColorLocal(Point argPoint) {
    return Color(argPoint.mbrX, argPoint.mbrY, argPoint.mbrZ);
}
void Pattern::setTransform(const Matrix &argMatrix) {
    mbrTransform = std::make_unique<Matrix>(argMatrix);
}

PatternStripe::PatternStripe() : Pattern() {
    mbrColors.push_back(mbrWhite);
    mbrColors.push_back(mbrBlack);
}
PatternStripe::PatternStripe(const PatternStripe& other) : Pattern()
{
    mbrColors = other.mbrColors;
    mbrTransform = std::make_unique<Matrix>(*other.mbrTransform.get());
}
PatternStripe::PatternStripe(Color argColorA, Color argColorB) : Pattern() {
    mbrColors.push_back(argColorA);
    mbrColors.push_back(argColorB);
}
Color PatternStripe::getColorLocal(Point argPoint) {
    return (int)std::floor(argPoint.mbrX) % 2 == 0 ? mbrColors[0] : mbrColors[1];
}

PatternGradient::PatternGradient() : Pattern() {
    mbrColors.push_back(mbrWhite);
    mbrColors.push_back(mbrBlack);
}
PatternGradient::PatternGradient(const PatternGradient& other) : Pattern()
{
    mbrColors = other.mbrColors;
    mbrTransform = std::make_unique<Matrix>(*other.mbrTransform.get());
}
PatternGradient::PatternGradient(Color argColorA, Color argColorB) : Pattern() {
    mbrColors.push_back(argColorA);
    mbrColors.push_back(argColorB);
}
Color PatternGradient::getColorLocal(Point argPoint) {
    return mbrColors[0] + (mbrColors[1]-mbrColors[0]) * (argPoint.mbrX - floor(argPoint.mbrX));
}

PatternRing::PatternRing() : Pattern() {
    mbrColors.push_back(mbrWhite);
    mbrColors.push_back(mbrBlack);
}
PatternRing::PatternRing(const PatternRing& other) : Pattern()
{
    mbrColors = other.mbrColors;
    mbrTransform = std::make_unique<Matrix>(*other.mbrTransform.get());
}
PatternRing::PatternRing(Color argColorA, Color argColorB) : Pattern() {
    mbrColors.push_back(argColorA);
    mbrColors.push_back(argColorB);
}
Color PatternRing::getColorLocal(Point argPoint) {
    return (int)fmod(floor(sqrt(pow(argPoint.mbrX,2) + pow(argPoint.mbrZ,2))),2) == 0 ? mbrColors[0]: mbrColors[1];
}

PatternChecker3d::PatternChecker3d() : Pattern() {
    mbrColors.push_back(mbrWhite);
    mbrColors.push_back(mbrBlack);
}
PatternChecker3d::PatternChecker3d(const PatternChecker3d& other) : Pattern()
{
    mbrColors = other.mbrColors;
    mbrTransform = std::make_unique<Matrix>(*other.mbrTransform.get());
}
PatternChecker3d::PatternChecker3d(Color argColorA, Color argColorB) : Pattern() {
    mbrColors.push_back(argColorA);
    mbrColors.push_back(argColorB);
}
Color PatternChecker3d::getColorLocal(Point argPoint) {
    return (int)fmod(floor(argPoint.mbrX) + floor(argPoint.mbrY) + floor(argPoint.mbrZ),2) == 0 ? mbrColors[0]: mbrColors[1];
}