<?xml version="1.0" ?>
<xsl:stylesheet version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:wix="http://schemas.microsoft.com/wix/2006/wi">


  <!-- Copy all attributes and elements to the output. -->
  <xsl:template match="@*|*">
    <xsl:copy>
      <xsl:apply-templates select="@*" />
      <xsl:apply-templates select="*" />
    </xsl:copy>
  </xsl:template>


  <xsl:output method="xml" indent="yes" />


  <!-- Set up keys for ignoring various file types -->
  <xsl:key name="ignored-files-search" match="wix:Component[contains(wix:File/@Source, '.vshost.exe')]" use="@Id" />
  <xsl:key name="ignored-files-search" match="wix:Component[contains(wix:File/@Source, '.pdb')]" use="@Id" />
  <xsl:key name="ignored-files-search" match="wix:Component[contains(wix:File/@Source, '.xml')]" use="@Id" />
  <xsl:key name="ignored-files-search" match="wix:Component[contains(wix:File/@Source, '.log')]" use="@Id" />
  <xsl:key name="ignored-directories-search" match="wix:Directory[@Name = 'logs']" use="descendant::wix:Component/@Id" />
  <xsl:key name="ignored-directories-search" match="wix:Directory[@Name = 'Profiles']" use="descendant::wix:Component/@Id" />

  <!-- Match and remove ignored files -->
  <xsl:template match="wix:Component[key('ignored-files-search', @Id)]" />
  <xsl:template match="wix:ComponentRef[key('ignored-files-search', @Id)]" />
  
  
  <!-- Match and remove ignored folders -->
  <xsl:template match="wix:Directory[@Name = 'logs']" />
  <xsl:template match="wix:Directory[@Name = 'Profiles']" />
  
  <xsl:template match="wix:ComponentRef[key('ignored-directories-search', @Id)]" />
  

  <!-- Copy all attributes and elements to the output. -->
  <xsl:template match="@*|*">
    <xsl:copy>
      <xsl:apply-templates select="@*" />
      <xsl:apply-templates select="*" />
    </xsl:copy>
  </xsl:template>
  
</xsl:stylesheet>
