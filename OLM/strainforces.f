      subroutine cstrain(nel,nnmax,nelmax,elnode,loads,elmat,xcoord,
     +  strain,eltype)
      implicit none
      integer i,nel,m,ii,jj,kk,nelmax,nnmax
      integer elnode(nelmax,*),eltype(*)
      double precision ue(4),loads(*),elmat(nelmax,*),strain(*),
     +  xcoord(nnmax,*),nlength,alpha
      double precision cstrainth,ey,eult,sult,Es
      
      do i=1,nel
         m=0
         do ii=1,2
         do jj=1,2
            m=m+1
            ue(m)=loads((elnode(i,ii)-1)*2+jj)  
         enddo
         enddo

         nlength=(xcoord(elnode(i,1),1)-xcoord(elnode(i,2),1))**2.
     +   +(xcoord(elnode(i,1),2)-xcoord(elnode(i,2),2))**2.
         nlength=dsqrt(nlength)
         strain(i)=(nlength-elmat(i,3))/elmat(i,3)
c         if(eltype(i).eq.2)then
c            strain(i)=strain(i)-epsh
c         endif
      enddo
      
      return
      end
      
      subroutine cds(nelmax,nn,nel,steel,damage,elnode,dso)
      implicit none
      integer nelmax,i,nn,nel,steel(*),damage(*),
     +  elnode(nelmax,*)
      double precision dso(*)
      
      do i=1,nn
        dso(i)=0
      enddo
      do i=1,nel
        if(steel(i).eq.0.and.damage(i).eq.0)then
            dso(elnode(i,1))=dso(elnode(i,1))+1
            dso(elnode(i,2))=dso(elnode(i,2))+1
        endif
      enddo
      
      end
